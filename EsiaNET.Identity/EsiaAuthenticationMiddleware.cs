// EsiaNET.Identity version 0.1
// 
// Home page: 
// Author: https://github.com/xeltan

using System;
using System.Globalization;
using EsiaNET.Identity.Properties;
using EsiaNET.Identity.Provider;
using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Infrastructure;
using Owin;

namespace EsiaNET.Identity
{
    public class EsiaAuthenticationMiddleware : AuthenticationMiddleware<EsiaAuthenticationOptions>
    {
        private readonly ILogger _logger;
        private readonly EsiaClient _esiaClient;

        public EsiaAuthenticationMiddleware(OwinMiddleware next, IAppBuilder app, EsiaAuthenticationOptions options) : base(next, options)
        {
            if ( String.IsNullOrWhiteSpace(Options.EsiaOptions.ClientId) )
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, Resources.ErrorParamMustBeProvided, "ClientId"));

            if ( String.IsNullOrWhiteSpace(Options.EsiaOptions.Scope) )
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, Resources.ErrorParamMustBeProvided, "Scope"));

            if ( String.IsNullOrWhiteSpace(Options.EsiaOptions.RequestType) )
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, Resources.ErrorParamMustBeProvided, "RequestType"));

            if ( Options.EsiaOptions.SignProvider == null )
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, Resources.ErrorParamMustBeProvided, "SignProvider"));

            _logger = app.CreateLogger<EsiaAuthenticationMiddleware>();

            if ( Options.DataFormat == null )
            {
                IDataProtector dataProtecter = app.CreateDataProtector(typeof(EsiaAuthenticationMiddleware).FullName, Options.AuthenticationType, "v1");

                Options.DataFormat = new PropertiesDataFormat(dataProtecter);
            }

            if ( Options.Provider == null )
            {
                Options.Provider = new EsiaAuthenticationProvider();
            }

            if ( String.IsNullOrEmpty(Options.SignInAsAuthenticationType) )
            {
                Options.SignInAsAuthenticationType = app.GetDefaultSignInAsAuthenticationType();
            }

            _esiaClient = new EsiaClient(Options.EsiaOptions);
        }

        protected override AuthenticationHandler<EsiaAuthenticationOptions> CreateHandler()
        {
            return new EsiaAuthenticationHandler(_esiaClient, _logger);
        }
    }
}