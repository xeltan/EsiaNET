// EsiaNET.Identity version 0.1
// 
// Home page: 
// Author: https://github.com/xeltan

using System;
using System.Threading.Tasks;

namespace EsiaNET.Identity.Provider
{
    /// <summary>
    /// Default <see cref="IEsiaAuthenticationProvider"/> implementation.
    /// </summary>
    public class EsiaAuthenticationProvider : IEsiaAuthenticationProvider
    {
        /// <summary>
        /// Initializes a new <see cref="EsiaAuthenticationProvider"/>
        /// </summary>
        public EsiaAuthenticationProvider()
        {
            OnAuthenticated = context => Task.FromResult<object>(null);
            OnReturnEndpoint = context => Task.FromResult<object>(null);
            OnApplyRedirect = context =>
                    context.Response.Redirect(context.RedirectUri);
        }

        /// <summary>
        /// Gets or sets the function that is invoked when the Authenticated method is invoked.
        /// </summary>
        public Func<EsiaAuthenticatedContext, Task> OnAuthenticated { get; set; }

        /// <summary>
        /// Gets or sets the function that is invoked when the ReturnEndpoint method is invoked.
        /// </summary>
        public Func<EsiaReturnEndpointContext, Task> OnReturnEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the delegate that is invoked when the ApplyRedirect method is invoked.
        /// </summary>
        public Action<EsiaApplyRedirectContext> OnApplyRedirect { get; set; }

        /// <summary>
        /// Invoked whenever ESIA succesfully authenticates a user
        /// </summary>
        /// <param name="context">Contains information about the login session as well as the user <see cref="System.Security.Claims.ClaimsIdentity"/>.</param>
        /// <returns>A <see cref="Task"/> representing the completed operation.</returns>
        public virtual Task Authenticated(EsiaAuthenticatedContext context)
        {
            return OnAuthenticated(context);
        }

        /// <summary>
        /// Invoked prior to the <see cref="System.Security.Claims.ClaimsIdentity"/> being saved in a local cookie and the browser being redirected to the originally requested URL.
        /// </summary>
        /// <param name="context">Contains information about the login session as well as the user <see cref="System.Security.Claims.ClaimsIdentity"/></param>
        /// <returns>A <see cref="Task"/> representing the completed operation.</returns>
        public virtual Task ReturnEndpoint(EsiaReturnEndpointContext context)
        {
            return OnReturnEndpoint(context);
        }

        /// <summary>
        /// Called when a Challenge causes a redirect to authorize endpoint in the ESIA account middleware
        /// </summary>
        /// <param name="context">Contains redirect URI and <see cref="Microsoft.Owin.Security.AuthenticationProperties"/> of the challenge </param>
        public virtual void ApplyRedirect(EsiaApplyRedirectContext context)
        {
            OnApplyRedirect(context);
        }
    }
}