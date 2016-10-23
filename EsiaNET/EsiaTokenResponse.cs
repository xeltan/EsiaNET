// EsiaNET version 0.1
// 
// Home page: 
// Author: https://github.com/xeltan

namespace EsiaNET
{
    /// <summary>
    /// Provides token response
    /// </summary>
    public class EsiaTokenResponse
    {
        /// <summary>
        /// Access token
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Refresh token
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// Expires in
        /// </summary>
        public string ExpiresIn { get; set; }
    }
}