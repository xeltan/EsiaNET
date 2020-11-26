using System;
using System.Globalization;
using System.Text;
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
            Parse(accessToken);
        }

        /// <summary>
        /// Initialize a new instance with access token parameters
        /// </summary>
        /// <param name="accessToken">Access token</param>
        /// <param name="refreshToken">Refresh token</param>
        /// <param name="expiresIn">Expires in</param>
        public EsiaToken(string accessToken, string refreshToken, string expiresIn) : this(accessToken)
        {
            RefreshToken = refreshToken;

            if ( Int32.TryParse(expiresIn, NumberStyles.Integer, CultureInfo.InvariantCulture, out int expiresValue) )
            {
                ExpiresIn = TimeSpan.FromSeconds(expiresValue);
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
        public DateTime? BeginDate { get; private set; }

        /// <summary>
        /// End date and time of access token
        /// </summary>
        public DateTime? EndDate { get; private set; }

        /// <summary>
        /// Create date and time of access token
        /// </summary>
        public DateTime? CreateDate { get; private set; }

        /// <summary>
        /// Token identifier
        /// </summary>
        public string Sid { get; private set; }

        /// <summary>
        /// Subject identifier (oid)
        /// </summary>
        public string SbjId { get; private set; }

        private void Parse(string accessToken)
        {
            string[] parts = accessToken.Split('.');
            string payload = Encoding.UTF8.GetString(Base64Decode(parts[1]));
            JObject payloadObject = JObject.Parse(payload);

            Sid = EsiaHelpers.PropertyValueIfExists("urn:esia:sid", payloadObject);
            SbjId = EsiaHelpers.PropertyValueIfExists("urn:esia:sbj_id", payloadObject);

            double seconds;
            string value = EsiaHelpers.PropertyValueIfExists("exp", payloadObject);

            if ( value != null && Double.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out seconds) )
            {
                EndDate = EsiaHelpers.DateFromUnixSeconds(seconds);
            }

            value = EsiaHelpers.PropertyValueIfExists("nbf", payloadObject);

            if ( value != null && Double.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out seconds) )
            {
                BeginDate = EsiaHelpers.DateFromUnixSeconds(seconds);
            }

            value = EsiaHelpers.PropertyValueIfExists("iat", payloadObject);

            if ( value != null && Double.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out seconds) )
            {
                CreateDate = EsiaHelpers.DateFromUnixSeconds(seconds);
            }
        }

        private static byte[] Base64Decode(string input)
        {
            input = input.Replace('-', '+').Replace('_', '/');

            switch ( input.Length % 4 )
            {
                case 0:
                    break;
                case 2:
                    input = String.Format("{0}==", input);
                    break;
                case 3:
                    input = String.Format("{0}=", input);
                    break;
                default:
                    throw new Exception("Illegal base64url string!");
            }

            return Convert.FromBase64String(input); // Standard base64 decoder
        }
    }
}