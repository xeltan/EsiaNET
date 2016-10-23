// EsiaNET.Identity version 0.1
// 
// Home page: 
// Author: https://github.com/xeltan

using System;
using Owin;

namespace EsiaNET.Identity
{
    public static class EsiaAuthenticationExtensions
    {
        /// <summary>
        /// Enables ESIA authentication middleware
        /// </summary>
        /// <param name="app">OWIN IAppBuilder instance</param>
        /// <param name="esiaOptions">ESIA authentication middleware options</param>
        /// <returns></returns>
        public static IAppBuilder UseEsiaAuthentication(this IAppBuilder app, EsiaAuthenticationOptions esiaOptions)
        {
            if ( app == null )
            {
                throw new ArgumentNullException(nameof(app));
            }

            if ( esiaOptions == null )
            {
                throw new ArgumentNullException(nameof(esiaOptions));
            }

            return app.Use(typeof(EsiaAuthenticationMiddleware), app, esiaOptions);
        }
    }
}