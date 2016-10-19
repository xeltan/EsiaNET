// EsiaNET version 0.1
// 
// Home page: 
// Author: https://github.com/xeltan

using System;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace EsiaNET
{
    public class EsiaToken
    {
        public EsiaToken(string accessToken)
        {
            if ( String.IsNullOrEmpty(accessToken) ) throw new ArgumentNullException("accessToken");

            AccessToken = accessToken;
        }

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

        public string AccessToken { get; }

        public string RefreshToken { get; }

        public TimeSpan? ExpiresIn { get; }

        public DateTime? BeginDate { get; }

        public DateTime? EndDate { get; }

        public DateTime? CreateDate { get; }

        public string Sid { get; }

        public string SbjId { get; }
    }
}