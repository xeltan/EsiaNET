// EsiaNET version 0.1
// 
// Home page: 
// Author: https://github.com/xeltan

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using EsiaNET.Properties;
using Newtonsoft.Json.Linq;

namespace EsiaNET
{
    [Flags]
    public enum SendStyles
    {
        None = 0,
        CheckTokenTime = 1,
        RefreshToken = 2,
        VerifyToken = 4,
        Normal = 7
    }

    public enum TokenRequest
    {
        ByAuthCode,
        ByRefresh,
        ByCredential
    }

    public class EsiaClient
    {
        private HttpClient _httpClient;

        public EsiaClient(EsiaOptions options)
        {
            if ( options == null ) throw new ArgumentNullException("options");

            Options = options;

            if ( String.IsNullOrWhiteSpace(Options.ClientId) )
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, Resources.ErrorParamMustBeProvided, "ClientId"));

            if ( String.IsNullOrWhiteSpace(Options.Scope) )
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, Resources.ErrorParamMustBeProvided, "Scope"));

            if ( String.IsNullOrWhiteSpace(Options.RequestType) )
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, Resources.ErrorParamMustBeProvided, "RequestType"));

            if ( Options.SignProvider == null )
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, Resources.ErrorParamMustBeProvided, "SignProvider"));
        }

        public EsiaClient(EsiaOptions options, string accessToken) : this(options, new EsiaToken(accessToken))
        {
        }

        public EsiaClient(EsiaOptions options, EsiaToken token) : this(options)
        {
            if ( token == null ) throw new ArgumentNullException("token");

            Token = token;
        }

        public EsiaOptions Options { get; }

        public EsiaToken Token { get; set; }

        public HttpStatusCode LastStatusCode { get; private set; }

        public string LastStatusMessage { get; private set; }

        protected HttpClient HttpClient
        {
            get { return _httpClient ?? (_httpClient = CreateHttpClient()); }
        }

        public virtual string BuildRedirectUri(string callbackUri = null)
        {
            if ( String.IsNullOrEmpty(callbackUri) ) callbackUri = Options.CallbackUri;

            string timestamp = DateTime.UtcNow.ToString("yyyy.MM.dd HH:mm:ss +0000");
            string state = Options.State.ToString("D");
            string clientSecret = BuildClientSecret(Options.Scope, timestamp, Options.ClientId, state);

            var builder = new StringBuilder();

            builder.Append(Options.RedirectUri);
            builder.AppendFormat("?client_id={0}", Uri.EscapeDataString(Options.ClientId));
            builder.AppendFormat("&scope={0}", Uri.EscapeDataString(Options.Scope));
            builder.AppendFormat("&response_type={0}", Uri.EscapeDataString(Options.RequestType));
            builder.AppendFormat("&state={0}", Uri.EscapeDataString(state));
            builder.AppendFormat("&timestamp={0}", Uri.EscapeDataString(timestamp));
            builder.AppendFormat("&access_type={0}", Uri.EscapeDataString(Options.AccessType == AccessType.Online ? "online" : "offline"));
            builder.AppendFormat("&redirect_uri={0}", Uri.EscapeDataString(callbackUri));
            builder.AppendFormat("&client_secret={0}", Uri.EscapeDataString(clientSecret));

            return builder.ToString();
        }

        public virtual async Task<EsiaTokenResponse> GetOAuthTokenAsync(string authCode, string callbackUri = null)
        {
            if ( String.IsNullOrEmpty(authCode) ) throw new ArgumentNullException("authCode");

            if ( String.IsNullOrEmpty(callbackUri) ) callbackUri = Options.CallbackUri;

            return await InternalGetOAuthTokenAsync(authCode, callbackUri, TokenRequest.ByAuthCode);
        }

        public virtual async Task<EsiaTokenResponse> GetOAuthTokenByRefreshAsync(string refreshToken, string callbackUri = null)
        {
            if ( String.IsNullOrEmpty(refreshToken) ) throw new ArgumentNullException("refreshToken");

            if ( String.IsNullOrEmpty(callbackUri) ) callbackUri = Options.CallbackUri;

            return await InternalGetOAuthTokenAsync(refreshToken, callbackUri, TokenRequest.ByRefresh);
        }

        public virtual async Task<EsiaTokenResponse> GetOAuthTokenByCredentialsAsync(string callbackUri = null)
        {
            if ( String.IsNullOrEmpty(callbackUri) ) callbackUri = Options.CallbackUri;

            return await InternalGetOAuthTokenAsync(String.Empty, callbackUri, TokenRequest.ByCredential);
        }

        protected virtual async Task<EsiaTokenResponse> InternalGetOAuthTokenAsync(string code, string callbackUri, TokenRequest request)
        {
            string timestamp = DateTime.UtcNow.ToString("yyyy.MM.dd HH:mm:ss +0000");
            string state = Options.State.ToString("D");
            string clientSecret = BuildClientSecret(Options.Scope, timestamp, Options.ClientId, state);
            string paramName;
            string paramValue;
            string grantType;

            if ( request == TokenRequest.ByRefresh )
            {
                paramName = "refresh_token";
                paramValue = code;
                grantType = "refresh_token";
            }
            else if ( request == TokenRequest.ByCredential )
            {
                paramName = "response_type";
                paramValue = "token";
                grantType = "client_credentials";
            }
            else
            {
                paramName = "code";
                paramValue = code;
                grantType = "authorization_code";
            }

            var requestParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("client_id", Options.ClientId),
                new KeyValuePair<string, string>(paramName, paramValue),
                new KeyValuePair<string, string>("grant_type", grantType),
                new KeyValuePair<string, string>("state", state),
                new KeyValuePair<string, string>("scope", Options.Scope),
                new KeyValuePair<string, string>("timestamp", timestamp),
                new KeyValuePair<string, string>("token_type", "Bearer"),
                new KeyValuePair<string, string>("client_secret", clientSecret)
            };

            if ( request != TokenRequest.ByCredential ) requestParams.Add(new KeyValuePair<string, string>("redirect_uri", callbackUri));

            var requestContent = new FormUrlEncodedContent(requestParams);
#if DEBUG
            var content = await requestContent.ReadAsStringAsync();

            Debug.Write(content);
#endif
            HttpResponseMessage response = await HttpClient.PostAsync(Options.TokenUri, requestContent);

            await UpdateLastStatusCode(response);

            response.EnsureSuccessStatusCode();

            string oauthTokenResponse = await response.Content.ReadAsStringAsync();
            JObject oauth2Token = JObject.Parse(oauthTokenResponse);
            string accessToken = oauth2Token["access_token"].Value<string>();

            if ( string.IsNullOrWhiteSpace(accessToken) )
            {
                throw new Exception("Access token was not found");
            }

            var value = oauth2Token.Value<string>("state");

            if ( value == null || value != state )
            {
                throw new Exception("State parameter is missing or invalid");
            }

            return new EsiaTokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = oauth2Token.Value<string>("refresh_token"),
                ExpiresIn = oauth2Token.Value<string>("expires_in")
            };
        }

        private async Task UpdateLastStatusCode(HttpResponseMessage response)
        {
            LastStatusCode = response.StatusCode;
            LastStatusMessage = null;

            if ( !response.IsSuccessStatusCode )
            {
                LastStatusMessage = await response.Content.ReadAsStringAsync();
            }
        }

        public virtual bool VerifyToken(string accessToken)
        {
            if ( String.IsNullOrEmpty(accessToken) ) throw new ArgumentNullException("accessToken");

            string[] parts = accessToken.Split('.');
            string header = Encoding.UTF8.GetString(Base64Decode(parts[0]));
            JObject headerObject = JObject.Parse(header);

            return parts.Length > 2 && VerifySignature(headerObject.Value<string>("alg"), String.Format("{0}.{1}", parts[0], parts[1]), parts[2]);
        }

        public static EsiaToken CreateToken(EsiaTokenResponse tokenResponse)
        {
            if ( tokenResponse == null ) throw new ArgumentNullException("tokenResponse");

            string[] parts = tokenResponse.AccessToken.Split('.');
            string payload = Encoding.UTF8.GetString(Base64Decode(parts[1]));
            JObject payloadObject = JObject.Parse(payload);

            return new EsiaToken(tokenResponse.AccessToken, tokenResponse.RefreshToken, tokenResponse.ExpiresIn, payloadObject);
        }

        public virtual async Task<HttpResponseMessage> SendAsync(HttpMethod method, string requestUri,
            SendStyles styles = SendStyles.Normal,
            IList<KeyValuePair<string, string>> requestParams = null, IList<KeyValuePair<string, string>> headers = null)
        {
            CheckTokenExist();

            DateTime currentTime = DateTime.Now;

            if ( styles.HasFlag(SendStyles.CheckTokenTime) && Token.BeginDate.HasValue && currentTime < Token.BeginDate )
                throw new Exception("Token start time has not come");

            if ( styles.HasFlag(SendStyles.CheckTokenTime) && Token.EndDate.HasValue && currentTime > Token.EndDate )
            {
                if ( styles.HasFlag(SendStyles.RefreshToken) && !String.IsNullOrEmpty(Token.RefreshToken) )
                {
                    await UpdateToken(styles);
                }
            }

            var response = await InternalSendAsync(method, requestUri, requestParams, headers);

            if ( response.StatusCode == HttpStatusCode.Unauthorized && styles.HasFlag(SendStyles.RefreshToken) )
            {
                await UpdateToken(styles);

                response = await InternalSendAsync(method, requestUri, requestParams, headers);
            }

            return response;
        }

        private void CheckTokenExist()
        {
            if ( Token == null ) throw new ArgumentNullException("Token");
        }

        public virtual async Task<string> GetAsync(string requestUri, SendStyles styles = SendStyles.Normal)
        {
            var response = await SendAsync(HttpMethod.Get, requestUri, styles);

            if ( !response.IsSuccessStatusCode ) return null;

            return await response.Content.ReadAsStringAsync();
        }

        public virtual async Task<string> PostAsync(string requestUri, SendStyles styles = SendStyles.Normal,
            IList<KeyValuePair<string, string>> requestParams = null)
        {
            var response = await SendAsync(HttpMethod.Post, requestUri, styles, requestParams);

            if ( !response.IsSuccessStatusCode ) return null;

            return await response.Content.ReadAsStringAsync();
        }

        public virtual async Task<string> PutAsync(string requestUri, SendStyles styles = SendStyles.Normal,
            IList<KeyValuePair<string, string>> requestParams = null)
        {
            var response = await SendAsync(HttpMethod.Put, requestUri, styles, requestParams);

            if ( !response.IsSuccessStatusCode ) return null;

            return await response.Content.ReadAsStringAsync();
        }

        private async Task UpdateToken(SendStyles styles)
        {
            var tokenResponse = await GetOAuthTokenByRefreshAsync(Token.RefreshToken);

            if ( styles.HasFlag(SendStyles.VerifyToken) && !VerifyToken(tokenResponse.AccessToken) ) throw new Exception("Token signature is invalid");

            Token = CreateToken(tokenResponse);
        }

        protected virtual async Task<HttpResponseMessage> InternalSendAsync(HttpMethod method, string requestUri,
            IList<KeyValuePair<string, string>> requestParams, IList<KeyValuePair<string, string>> headers)
        {
            var message = new HttpRequestMessage(method, requestUri);

            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token.AccessToken);
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if ( headers != null )
            {
                foreach ( var header in headers )
                {
                    message.Headers.Add(header.Key, header.Value);
                }
            }

            if ( requestParams != null ) message.Content = new FormUrlEncodedContent(requestParams);

            var response = await HttpClient.SendAsync(message);

            await UpdateLastStatusCode(response);

            return response;
        }

        private HttpClient CreateHttpClient()
        {
            return new HttpClient
            {
                Timeout = Options.BackchannelTimeout,
                MaxResponseContentBufferSize = 1024 * 1024 * 10 // 10 MB
            };
        }

        private string BuildClientSecret(string scope, string timestamp, string clientId, string state)
        {
            string signMessage = $"{scope}{timestamp}{clientId}{state}";

            byte[] bytes = Encoding.UTF8.GetBytes(signMessage);
            var certificate = Options.SignProvider.GetCertificate();

            if ( certificate == null ) throw new ArgumentException(Resources.ErrorCertificateMustBeProvided);

            byte[] encodedSignature = Options.SignProvider.SignMessage(bytes, certificate);

            return Base64UrlEncode(encodedSignature);
        }

        private string Base64UrlEncode(byte[] input)
        {
            if ( input == null ) throw new ArgumentNullException(nameof(input));

            if ( input.Length < 1 ) return String.Empty;

            string base64Str = null;
            int endPos = 0;
            char[] base64Chars = null;

            base64Str = Convert.ToBase64String(input);

            for ( endPos = base64Str.Length; endPos > 0; endPos-- )
            {
                if ( base64Str[endPos - 1] != '=' )
                {
                    break;
                }
            }

            base64Chars = new char[endPos + 1];
            base64Chars[endPos] = (char)((int)'0' + base64Str.Length - endPos);

            for ( int iter = 0; iter < endPos; iter++ )
            {
                char c = base64Str[iter];

                switch ( c )
                {
                    case '+':
                        base64Chars[iter] = '-';
                        break;

                    case '/':
                        base64Chars[iter] = '_';
                        break;

                    case '=':
                        base64Chars[iter] = c;
                        break;

                    default:
                        base64Chars[iter] = c;
                        break;
                }
            }

            return new string(base64Chars);
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

        private bool VerifySignature(string alg, string message, string signature)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            byte[] signatureBytes = Base64Decode(signature);
            var certificate = Options.SignProvider.GetEsiaCertificate();

            if ( certificate == null ) throw new ArgumentException(Resources.ErrorCertificateMustBeProvided);

            return Options.SignProvider.VerifyMessage(alg, bytes, signatureBytes, certificate);
        }
    }
}