// EsiaNET.Identity version 0.1
// 
// Home page: 
// Author: https://github.com/xeltan

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EsiaNET.Identity.Provider;
using Microsoft.Owin;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;

// ReSharper disable PossibleMultipleEnumeration

namespace EsiaNET.Identity
{
    public class EsiaAuthenticationHandler : AuthenticationHandler<EsiaAuthenticationOptions>
    {
        private readonly ILogger _logger;
        private readonly EsiaClient _esiaClient;

        public EsiaAuthenticationHandler(EsiaClient esiaClient, ILogger logger)
        {
            _esiaClient = esiaClient;
            _logger = logger;
        }

        protected override async Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            AuthenticationProperties properties = null;

            try
            {
                IReadableStringCollection query = Request.Query;

                properties = UnpackDataParameter(query);

                if ( properties == null )
                {
                    _logger.WriteWarning("Invalid return data");

                    return null;
                }

                // Anti-CSRF
                if ( !ValidateCorrelationId(properties, _logger) )
                {
                    return new AuthenticationTicket(null, properties);
                }

                var queryValues = await ParseRequestAsync(query);
                string value = queryValues["error"];

                if ( value != null )
                {
                    var description = queryValues["error_description"];

                    _logger.WriteWarning($"ESIA Error '{value}'. {description}");

                    return new AuthenticationTicket(null, properties);
                }

                value = queryValues["state"];

                if ( value == null || value != Options.EsiaOptions.State.ToString("D") )
                {
                    _logger.WriteWarning("State parameter is missing or invalid");

                    return new AuthenticationTicket(null, properties);
                }

                string authCode = queryValues["code"];

                if ( String.IsNullOrWhiteSpace(authCode) )
                {
                    _logger.WriteWarning("Code parameter is missing or invalid");

                    return new AuthenticationTicket(null, properties);
                }

                var tokenResponse = await _esiaClient.GetOAuthTokenAsync(authCode, BuildCallbackUri(""));

                if ( Options.VerifyTokenSignature && !_esiaClient.VerifyToken(tokenResponse.AccessToken) )
                {
                    _logger.WriteWarning("Token signature is invalid");

                    return new AuthenticationTicket(null, properties);
                }

                _esiaClient.Token = EsiaClient.CreateToken(tokenResponse);

                var personInfo = await _esiaClient.GetPersonInfoAsync(SendStyles.None);
                var contactsInfo = await _esiaClient.GetPersonContactsAsync(SendStyles.None);

                var context = new EsiaAuthenticatedContext(Context, _esiaClient.Token, personInfo, contactsInfo);

                context.Identity = new ClaimsIdentity(
                    new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, context.Id, ClaimValueTypes.String, Options.AuthenticationType),
                        new Claim("urn:esia:sbj_id", context.Id, ClaimValueTypes.String, Options.AuthenticationType),
                        new Claim(ClaimTypes.AuthenticationMethod, "ESIA", ClaimValueTypes.String, Options.AuthenticationType)
                    },
                    Options.AuthenticationType,
                    ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);

                if ( personInfo != null )
                {
                    context.Identity.AddClaim(new Claim(ClaimTypes.Name, personInfo.Name, ClaimValueTypes.String, Options.AuthenticationType));
                    context.Identity.AddClaim(new Claim("urn:esia:trusted", personInfo.Trusted.ToString(), ClaimValueTypes.Boolean, Options.AuthenticationType));
                }

                if ( contactsInfo != null )
                {
                    var email = contactsInfo.FirstOrDefault(c => c.ContactType == ContactType.Email);

                    if ( email != null )
                        context.Identity.AddClaim(new Claim(ClaimTypes.Email, email.Value, ClaimValueTypes.String, Options.AuthenticationType));
                }

                await Options.Provider.Authenticated(context);

                context.Properties = properties;

                return new AuthenticationTicket(context.Identity, context.Properties);
            }
            catch ( Exception ex )
            {
                _logger.WriteError("Authentication failed", ex);

                return new AuthenticationTicket(null, properties);
            }
        }

        protected override Task ApplyResponseGrantAsync()
        {
            return Task.FromResult<object>(null);
        }

        protected override Task ApplyResponseChallengeAsync()
        {
            if ( Response.StatusCode != 401 )
            {
                return Task.FromResult<object>(null);
            }

            AuthenticationResponseChallenge challenge = Helper.LookupChallenge(Options.AuthenticationType, Options.AuthenticationMode);

            if ( challenge != null )
            {
                string requestPrefix = Request.Scheme + Uri.SchemeDelimiter + Request.Host;
                var properties = challenge.Properties;

                if ( String.IsNullOrEmpty(properties.RedirectUri) )
                {
                    properties.RedirectUri = requestPrefix + Request.PathBase + Request.Path + Request.QueryString;
                }

                // Anti-CSRF
                GenerateCorrelationId(properties);

                var redirectUri = _esiaClient.BuildRedirectUri(BuildCallbackUri(Options.DataFormat.Protect(properties)));
                var redirectContext = new EsiaApplyRedirectContext(Context, Options, properties, redirectUri);

                Options.Provider.ApplyRedirect(redirectContext);
            }

            return Task.FromResult<object>(null);
        }

        public override async Task<bool> InvokeAsync()
        {
            if ( !String.IsNullOrEmpty(Options.EsiaOptions.CallbackUri) && Options.EsiaOptions.CallbackUri == Request.Path.ToString() )
            {
                return await InvokeReturnPathAsync();
            }

            return false;
        }

        private async Task<bool> InvokeReturnPathAsync()
        {
            AuthenticationTicket ticket = await AuthenticateAsync();

            if ( ticket == null )
            {
                _logger.WriteWarning("Invalid return state, unable to redirect.");
                Response.StatusCode = 500;

                return true;
            }

            var context = new EsiaReturnEndpointContext(Context, ticket)
            {
                SignInAsAuthenticationType = Options.SignInAsAuthenticationType,
                RedirectUri = ticket.Properties.RedirectUri
            };

            ticket.Properties.RedirectUri = null;

            await Options.Provider.ReturnEndpoint(context);

            if ( context.SignInAsAuthenticationType != null && context.Identity != null )
            {
                ClaimsIdentity signInIdentity = context.Identity;

                if ( !string.Equals(signInIdentity.AuthenticationType, context.SignInAsAuthenticationType, StringComparison.Ordinal) )
                {
                    signInIdentity = new ClaimsIdentity(signInIdentity.Claims, context.SignInAsAuthenticationType, signInIdentity.NameClaimType,
                        signInIdentity.RoleClaimType);
                }

                Context.Authentication.SignIn(context.Properties, signInIdentity);
            }

            if ( !context.IsRequestCompleted && context.RedirectUri != null )
            {
                if ( context.Identity == null )
                {
                    // add a redirect hint that sign-in failed in some way
                    context.RedirectUri = WebUtilities.AddQueryString(context.RedirectUri, "error", "access_denied");
                }

                Response.Redirect(context.RedirectUri);
                context.RequestCompleted();
            }

            return context.IsRequestCompleted;
        }

        private string BuildCallbackUri(string properties)
        {
            return $"{Request.Scheme}://{Request.Host}{RequestPathBase}{Options.EsiaOptions.CallbackUri}?data={Uri.EscapeDataString(properties)}";
        }

        private static string GetDataParameter(IReadableStringCollection query)
        {
            IList<string> values = query.GetValues("data");

            if ( values != null && values.Count == 1 )
            {
                return values[0];
            }

            return null;
        }

        private AuthenticationProperties UnpackDataParameter(IReadableStringCollection query)
        {
            string data = GetDataParameter(query);

            if ( data != null )
            {
                return Options.DataFormat.Unprotect(data);
            }

            return null;
        }

        private async Task<IReadableStringCollection> ParseRequestAsync(IReadableStringCollection query)
        {
            if ( Request.Method == "POST" )
            {
                IFormCollection form = await Request.ReadFormAsync();

                return form;
            }

            return query;
        }
    }
}