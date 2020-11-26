using System;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;

namespace EsiaNET.AspNetCore.Authentication
{
    public class EsiaAuthenticationOptions : OAuthOptions
    {
        /// <summary>
        ///     Initialize a new instance with ESIA client options passed to EsiaClient instance
        /// </summary>
        public EsiaAuthenticationOptions()
        {
            CallbackPath = new PathString("/esia-signin");
            State = Guid.NewGuid();
            AccessType = AccessType.Online;
            VerifyTokenSignature = false;
            RestOptions = new EsiaRestOptions();
        }

        /// <summary>
        ///     State parameter. Default is new Guid
        /// </summary>
        public Guid State { get; set; }

        /// <summary>
        ///     Access type parameter. Default: AccessType.Online
        /// </summary>
        public AccessType AccessType { get; set; }

        /// <summary>
        ///     True if middleware needs to verify token signature; otherwise, false
        /// </summary>
        public bool VerifyTokenSignature { get; set; }

        /// <summary>
        ///     Sign provider to get client system and ESIA certificates. Required
        /// </summary>
        public ISignProvider SignProvider { get; set; }

        public EsiaRestOptions RestOptions { get; set; }

        public override void Validate()
        {
            if ( String.IsNullOrEmpty(ClientId) )
                throw new ArgumentException(
                    String.Format(CultureInfo.CurrentCulture, Resources.Exception_OptionMustBeProvided,
                        "ClientId"), "ClientId");

            if ( String.IsNullOrEmpty(AuthorizationEndpoint) )
                throw new ArgumentException(
                    String.Format(CultureInfo.CurrentCulture, Resources.Exception_OptionMustBeProvided,
                        "AuthorizationEndpoint"), "AuthorizationEndpoint");

            if ( String.IsNullOrEmpty(TokenEndpoint) )
                throw new ArgumentException(
                    String.Format(CultureInfo.CurrentCulture, Resources.Exception_OptionMustBeProvided,
                        "TokenEndpoint"), "TokenEndpoint");

            if ( !CallbackPath.HasValue )
                throw new ArgumentException(
                    String.Format(CultureInfo.CurrentCulture, Resources.Exception_OptionMustBeProvided,
                        "CallbackPath"), "CallbackPath");

            if ( SignProvider == null )
                throw new ArgumentException(
                    String.Format(CultureInfo.CurrentCulture, Resources.Exception_OptionMustBeProvided,
                        "SignProvider"), "SignProvider");
        }

        /// <summary>
        ///     Creates default sign provider for custom handlers
        /// </summary>
        /// <param name="action">Handler that returns client system certificate registered with ESIA</param>
        /// <param name="esiaAction">Handler that returns ESIA certificate. Used only with token verification</param>
        /// <returns>Default sign provider with handlers</returns>
        public static ISignProvider CreateSignProvider(Func<X509Certificate2> action, Func<X509Certificate2> esiaAction = null)
        {
            if ( action == null ) throw new ArgumentException("Get certificate action must be provided");

            return new DefaultSignProvider(action, esiaAction);
        }
    }
}