// EsiaNET version 0.1
// 
// Home page: 
// Author: https://github.com/xeltan

using System;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace EsiaNET
{
    /// <summary>
    /// Provides ESIA access token
    /// </summary>
    public class EsiaToken
    {
        /// <summary>
        /// Initialize a new instance with access token string
        /// </summary>
        /// <param name="accessToken">access token</param>
        public EsiaToken(string accessToken)
        {
            if ( String.IsNullOrEmpty(accessToken) ) throw new ArgumentNullException("accessToken");

            AccessToken = accessToken;
        }

        /// <summary>
        /// Initialize a new instance with access token parameters
        /// </summary>
        /// <param name="accessToken">Access token</param>
        /// <param name="refreshToken">Refresh token</param>
        /// <param name="expiresIn">Expires in</param>
        /// <param name="payload">Payload object to parse</param>
        public EsiaToken(string accessToken, string refreshToken, string expiresIn, JObject payload) : this(accessToken)
        {
            RefreshToken = refreshToken;

            int expiresValue;

            if ( Int32.TryParse(expiresIn, NumberStyles.Integer, CultureInfo.InvariantCulture, out expiresValue) )
            {
                ExpiresIn = TimeSpan.FromSeconds(expiresValue);
            }

            if ( payload != null )
            {
                Sid = EsiaHelpers.PropertyValueIfExists("urn:esia:sid", payload);
                SbjId = EsiaHelpers.PropertyValueIfExists("urn:esia:sbj_id", payload);

                double seconds;
                string value = EsiaHelpers.PropertyValueIfExists("exp", payload);

                if ( value != null && Double.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out seconds) )
                {
                    EndDate = EsiaHelpers.DateFromUnixSeconds(seconds);
                }

                value = EsiaHelpers.PropertyValueIfExists("nbf", payload);

                if ( value != null && Double.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out seconds) )
                {
                    BeginDate = EsiaHelpers.DateFromUnixSeconds(seconds);
                }

                value = EsiaHelpers.PropertyValueIfExists("iat", payload);

                if ( value != null && Double.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out seconds) )
                {
                    CreateDate = EsiaHelpers.DateFromUnixSeconds(seconds);
                }
            }
        }

        /// <summary>
        /// Access token
        /// </summary>
        public string AccessToken { get; }

        /// <summary>
        /// Refresh token
        /// </summary>
        public string RefreshToken { get; }

        /// <summary>
        /// Expires in seconds
        /// </summary>
        public TimeSpan? ExpiresIn { get; }

        /// <summary>
        /// Begin date and time of access token
        /// </summary>
        public DateTime? BeginDate { get; }

        /// <summary>
        /// End date and time of access token
        /// </summary>
        public DateTime? EndDate { get; }

        /// <summary>
        /// Create date and time of access token
        /// </summary>
        public DateTime? CreateDate { get; }

        /// <summary>
        /// Token identifier
        /// </summary>
        public string Sid { get; }

        /// <summary>
        /// Subject identifier (oid)
        /// </summary>
        public string SbjId { get; }
    }
}