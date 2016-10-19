// EsiaNET.Identity version 0.1
// 
// Home page: 
// Author: https://github.com/xeltan

using System.Threading.Tasks;

namespace EsiaNET.Identity.Provider
{
    /// <summary>
    /// Specifies callback methods which the <see cref="EsiaAuthenticationMiddleware"></see> invokes to enable developer control over the authentication process. />
    /// </summary>
    public interface IEsiaAuthenticationProvider
    {
        /// <summary>
        /// Invoked whenever ESIA succesfully authenticates a user
        /// </summary>
        /// <param name="context">Contains information about the login session as well as the user <see cref="System.Security.Claims.ClaimsIdentity"/>.</param>
        /// <returns>A <see cref="Task"/> representing the completed operation.</returns>
        Task Authenticated(EsiaAuthenticatedContext context);

        /// <summary>
        /// Invoked prior to the <see cref="System.Security.Claims.ClaimsIdentity"/> being saved in a local cookie and the browser being redirected to the originally requested URL.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>A <see cref="Task"/> representing the completed operation.</returns>
        Task ReturnEndpoint(EsiaReturnEndpointContext context);

        /// <summary>
        /// Called when a Challenge causes a redirect to authorize endpoint in the ESIA middleware
        /// </summary>
        /// <param name="context">Contains redirect URI and <see cref="Microsoft.Owin.Security.AuthenticationProperties"/> of the challenge </param>
        void ApplyRedirect(EsiaApplyRedirectContext context);
    }
}