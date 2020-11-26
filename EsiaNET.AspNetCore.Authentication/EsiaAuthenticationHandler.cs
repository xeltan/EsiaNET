using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace EsiaNET.AspNetCore.Authentication
{
    public class EsiaAuthenticationHandler : OAuthHandler<EsiaAuthenticationOptions>
    {
        private readonly IEsiaAuthenticationService _authenticationService;

        public EsiaAuthenticationHandler(IEsiaAuthenticationService authenticationService,
            IOptionsMonitor<EsiaAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _authenticationService = authenticationService;
        }

        protected override async Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
        {
            var query = Request.Query;

            var data = query["data"];
            var properties = Options.StateDataFormat.Unprotect(data);

            if ( properties == null ) return HandleRequestResult.Fail("The oauth data was missing or invalid.");

            // OAuth2 10.12 CSRF
            if ( !ValidateCorrelationId(properties) ) return HandleRequestResult.Fail("Correlation failed.", properties);

            var error = query["error"];
            if ( !StringValues.IsNullOrEmpty(error) )
            {
                // Note: access_denied errors are special protocol errors indicating the user didn't
                // approve the authorization demand requested by the remote authorization server.
                // Since it's a frequent scenario (that is not caused by incorrect configuration),
                // denied errors are handled differently using HandleAccessDeniedErrorAsync().
                // Visit https://tools.ietf.org/html/rfc6749#section-4.1.2.1 for more information.
                var errorDescription = query["error_description"];
                var errorUri = query["error_uri"];
                if ( StringValues.Equals(error, "access_denied") )
                {
                    var result = await HandleAccessDeniedErrorAsync(properties);
                    if ( !result.None ) return result;

                    var deniedEx = new Exception("Access was denied by the resource owner or by the remote server.");
                    deniedEx.Data["error"] = error.ToString();
                    deniedEx.Data["error_description"] = errorDescription.ToString();
                    deniedEx.Data["error_uri"] = errorUri.ToString();

                    return HandleRequestResult.Fail(deniedEx, properties);
                }

                var failureMessage = new StringBuilder();
                failureMessage.Append(error);
                if ( !StringValues.IsNullOrEmpty(errorDescription) ) failureMessage.Append(";Description=").Append(errorDescription);

                if ( !StringValues.IsNullOrEmpty(errorUri) ) failureMessage.Append(";Uri=").Append(errorUri);

                var ex = new Exception(failureMessage.ToString());
                ex.Data["error"] = error.ToString();
                ex.Data["error_description"] = errorDescription.ToString();
                ex.Data["error_uri"] = errorUri.ToString();

                return HandleRequestResult.Fail(ex, properties);
            }

            var state = query["state"];

            // Checking response state with request state (from options)
            if ( StringValues.IsNullOrEmpty(state) || state != Options.State.ToString("D") )
                return HandleRequestResult.Fail("State parameter is missing or invalid.", properties);

            var code = query["code"];

            if ( StringValues.IsNullOrEmpty(code) ) return HandleRequestResult.Fail("Code was not found.", properties);

            var codeExchangeContext = new OAuthCodeExchangeContext(properties, code, BuildRedirectUri(Options.CallbackPath));
            using var tokens = await ExchangeCodeAsync(codeExchangeContext);

            if ( tokens.Error != null ) return HandleRequestResult.Fail(tokens.Error, properties);

            if ( String.IsNullOrEmpty(tokens.AccessToken) ) return HandleRequestResult.Fail("Failed to retrieve access token.", properties);

            // Check token signature if Options.VerifyTokenSignature is true
            if ( Options.VerifyTokenSignature && !_authenticationService.VerifyToken(tokens.AccessToken) )
                return HandleRequestResult.Fail("Token signature is invalid.", properties);

            var identity = new ClaimsIdentity(ClaimsIssuer);

            if ( Options.SaveTokens )
            {
                var authTokens = new List<AuthenticationToken>();

                authTokens.Add(new AuthenticationToken { Name = "access_token", Value = tokens.AccessToken });
                if ( !String.IsNullOrEmpty(tokens.RefreshToken) )
                    authTokens.Add(new AuthenticationToken { Name = "refresh_token", Value = tokens.RefreshToken });

                if ( !String.IsNullOrEmpty(tokens.TokenType) )
                    authTokens.Add(new AuthenticationToken { Name = "token_type", Value = tokens.TokenType });

                if ( !String.IsNullOrEmpty(tokens.ExpiresIn) )
                {
                    int value;
                    if ( Int32.TryParse(tokens.ExpiresIn, NumberStyles.Integer, CultureInfo.InvariantCulture, out value) )
                    {
                        // https://www.w3.org/TR/xmlschema-2/#dateTime
                        // https://msdn.microsoft.com/en-us/library/az4se3k1(v=vs.110).aspx
                        var expiresAt = Clock.UtcNow + TimeSpan.FromSeconds(value);
                        authTokens.Add(new AuthenticationToken
                        {
                            Name = "expires_at",
                            Value = expiresAt.ToString("o", CultureInfo.InvariantCulture)
                        });
                    }
                }

                properties.StoreTokens(authTokens);
            }

            var ticket = await CreateTicketAsync(identity, properties, tokens);

            if ( ticket != null )
                return HandleRequestResult.Success(ticket);
            return HandleRequestResult.Fail("Failed to retrieve user information from remote server.", properties);
        }

        protected override async Task<AuthenticationTicket> CreateTicketAsync(ClaimsIdentity identity, AuthenticationProperties properties,
            OAuthTokenResponse tokens)
        {
            var esiaClient = new EsiaClient(Options.RestOptions, tokens.AccessToken, Backchannel);

            // Get owner personal information and contacts. If requested scope does not allow some information, corresponding properties will be null
            var personInfo = await esiaClient.GetPersonInfoAsync();
            var contactsInfo = await esiaClient.GetPersonContactsAsync();

            identity.AddClaims(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, esiaClient.Token.SbjId, ClaimValueTypes.String, Scheme.Name),
                new Claim("urn:esia:sbj_id", esiaClient.Token.SbjId, ClaimValueTypes.String, Scheme.Name),
                new Claim(ClaimTypes.AuthenticationMethod, "ESIA", ClaimValueTypes.String, Scheme.Name)
            });

            if ( personInfo != null )
            {
                // Set some claims from personal info
                identity.AddClaim(new Claim(ClaimTypes.Name, personInfo.Name, ClaimValueTypes.String, Scheme.Name));
                identity.AddClaim(new Claim("urn:esia:trusted", personInfo.Trusted.ToString(), ClaimValueTypes.Boolean, Scheme.Name));
            }

            if ( contactsInfo != null )
            {
                // Set email claim from contacts
                var email = contactsInfo.FirstOrDefault(c => c.ContactType == ContactType.Email);

                if ( email != null )
                    identity.AddClaim(new Claim(ClaimTypes.Email, email.Value, ClaimValueTypes.String, Scheme.Name));
            }

            var context = new OAuthCreatingTicketContext(new ClaimsPrincipal(identity), properties, Context, Scheme, Options, Backchannel,
                tokens, new JsonElement());

            await Events.CreatingTicket(context);

            return new AuthenticationTicket(context.Principal, context.Properties, Scheme.Name);
        }

        protected override Task<OAuthTokenResponse> ExchangeCodeAsync(OAuthCodeExchangeContext context)
        {
            return _authenticationService.GetOAuthTokenAsync(Backchannel, context.Code, context.RedirectUri, Context.RequestAborted);
        }

        protected override string BuildChallengeUrl(AuthenticationProperties properties, string redirectUri)
        {
            var parameters = new Dictionary<string, string>
            {
                { "data", Options.StateDataFormat.Protect(properties) }
            };

            string callbackUri = QueryHelpers.AddQueryString(redirectUri, parameters);

            return _authenticationService.BuildRedirectUri(callbackUri);
        }
    }
}