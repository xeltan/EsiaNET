using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace EsiaNET.AspNetCore.Authentication
{
    public interface IEsiaAuthenticationService
    {
        /// <summary>
        ///     Builds a uri to ESIA auth service. The user must be redirected to this uri by the client system
        /// </summary>
        /// <param name="callbackUri">Callback uri for ESIA to return after passing authentication</param>
        /// <returns>Redirect uri</returns>
        string BuildRedirectUri(string callbackUri = null);

        /// <summary>
        ///     Returns response for access token by authorization code
        /// </summary>
        /// <param name="httpClient">Http client to send request.</param>
        /// <param name="authCode">Authorization code</param>
        /// <param name="callbackUri">Callback uri for ESIA.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Token response</returns>
        Task<OAuthTokenResponse> GetOAuthTokenAsync(HttpClient httpClient, string authCode, string callbackUri = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Returns response for access token by refresh token
        /// </summary>
        /// <param name="httpClient">Http client to send request.</param>
        /// <param name="refreshToken">Refresh token</param>
        /// <param name="callbackUri">Callback uri for ESIA.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Token response</returns>
        Task<OAuthTokenResponse> GetOAuthTokenByRefreshAsync(HttpClient httpClient, string refreshToken, string callbackUri = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Returns response for access token by client credential
        /// </summary>
        /// <param name="httpClient">Http client to send request.</param>
        /// <param name="callbackUri">Callback uri for ESIA.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Token response</returns>
        Task<OAuthTokenResponse> GetOAuthTokenByCredentialsAsync(HttpClient httpClient, string callbackUri = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Verifies access token signature
        /// </summary>
        /// <param name="accessToken">Access token</param>
        /// <returns>True if valid; otherwise, false</returns>
        bool VerifyToken(string accessToken);
    }
}