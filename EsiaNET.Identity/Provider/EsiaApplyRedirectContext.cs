// EsiaNET.Identity version 0.1
// 
// Home page: 
// Author: https://github.com/xeltan

using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Provider;

namespace EsiaNET.Identity.Provider
{
    public class EsiaApplyRedirectContext : BaseContext<EsiaAuthenticationOptions>
    {
        public EsiaApplyRedirectContext(IOwinContext context, EsiaAuthenticationOptions options,
            AuthenticationProperties properties, string redirectUri) : base(context, options)
        {
            RedirectUri = redirectUri;
            Properties = properties;
        }

        public string RedirectUri { get; private set; }

        /// <summary>
        /// Gets the authenticaiton properties of the challenge
        /// </summary>
        public AuthenticationProperties Properties { get; private set; }
    }
}