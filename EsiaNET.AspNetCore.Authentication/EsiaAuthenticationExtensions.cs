using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EsiaNET.AspNetCore.Authentication
{
    public static class EsiaAuthenticationExtensions
    {
        public static AuthenticationBuilder AddEsia(this AuthenticationBuilder builder)
        {
            return builder.AddEsia(EsiaDefaults.AuthenticationScheme, _ => { });
        }

        public static AuthenticationBuilder AddEsia(this AuthenticationBuilder builder,
            Action<EsiaAuthenticationOptions> configureOptions)
        {
            return builder.AddEsia(EsiaDefaults.AuthenticationScheme, configureOptions);
        }

        public static AuthenticationBuilder AddEsia(this AuthenticationBuilder builder, string authenticationScheme,
            Action<EsiaAuthenticationOptions> configureOptions)
        {
            return builder.AddEsia(authenticationScheme, EsiaDefaults.DisplayName, configureOptions);
        }

        public static AuthenticationBuilder AddEsia(this AuthenticationBuilder builder, string authenticationScheme, string displayName,
            Action<EsiaAuthenticationOptions> configureOptions)
        {
            builder.Services.TryAddScoped<IEsiaAuthenticationService, EsiaAuthenticationService>();

            return builder.AddOAuth<EsiaAuthenticationOptions, EsiaAuthenticationHandler>(authenticationScheme, displayName,
                configureOptions);
        }
    }
}