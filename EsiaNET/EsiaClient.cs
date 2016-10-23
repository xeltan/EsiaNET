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

        /// <summary>
        /// Check token expiration time
        /// </summary>
        CheckTokenTime = 1,

        /// <summary>
        /// Need to update access token by refresh token, if response is unauthorized
        /// </summary>
        RefreshToken = 2,

        /// <summary>
        /// Need to verify access token signature
        /// </summary>
        VerifyToken = 4,

        /// <summary>
        /// CheckTokenTime | RefreshToken | VerifyToken
        /// Check token expiration time
        /// And need to update access token by refresh token, if response is unauthorized
        /// And need to verify access token signature
        /// </summary>
        Normal = 7
    }

    /// <summary>
    /// Token request type enumeration
    /// </summary>
    public enum TokenRequest
    {
        ByAuthCode,
        ByRefresh,
        ByCredential
    }

    /// <summary>
    /// Provides a base class for communication with ESIA services
    /// </summary>
    public class EsiaClient
    {
        private HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of EsiaClient class with options 
        /// </summary>
        /// <param name="options">ESIA options class instance</param>
        public EsiaClient(EsiaOptions options)
        {
            if ( options == null ) throw new ArgumentNullException("options");

            Options = options;

            // Options.ClientId must be provided
            if ( String.IsNullOrWhiteSpace(Options.ClientId) )
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, Resources.ErrorParamMustBeProvided, "ClientId"));

            // Options.Scope must be provided
            if ( String.IsNullOrWhiteSpace(Options.Scope) )
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, Resources.ErrorParamMustBeProvided, "Scope"));

            // Options.RequestType must be provided
            if ( String.IsNullOrWhiteSpace(Options.RequestType) )
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, Resources.ErrorParamMustBeProvided, "RequestType"));

            // Options.SignProvider must be provided
            if ( Options.SignProvider == null )
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, Resources.ErrorParamMustBeProvided, "SignProvider"));
        }

        /// <summary>
        /// Initializes a new instance of EsiaClient class with options and access token
        /// </summary>
        /// <param name="options">ESIA options class instance</param>
        /// <param name="accessToken">Access token</param>
        public EsiaClient(EsiaOptions options, string accessToken) : this(options, new EsiaToken(accessToken))
        {
        }

        /// <summary>
        /// Initializes a new instance of EsiaClient class with options and access token
        /// </summary>
        /// <param name="options">ESIA options class instance</param>
        /// <param name="token">Instance of EsiaToken class</param>
        public EsiaClient(EsiaOptions options, EsiaToken token) : this(options)
        {
            if ( token == null ) throw new ArgumentNullException("token");

            Token = token;
        }

        /// <summary>
        /// ESIA Options
        /// </summary>
        public EsiaOptions Options { get; }

        /// <summary>
        /// Access token to communicate with ESIA REST services
        /// </summary>
        public EsiaToken Token { get; set; }

        /// <summary>
        /// Last send operation http status code
        /// </summary>
        public HttpStatusCode LastStatusCode { get; private set; }

        /// <summary>
        /// Last send operation status message
        /// </summary>
        public string LastStatusMessage { get; private set; }

        /// <summary>
        /// Instance of HttpClient class for sending request and receiving response from ESIA resource
        /// </summary>
        protected HttpClient HttpClient
        {
            get { return _httpClient ?? (_httpClient = CreateHttpClient()); }
        }

        /// <summary>
        /// Builds a uri to ESIA auth service. The user must be redirected to this uri by the client system
        /// </summary>
        /// <param name="callbackUri">Callback uri for ESIA to return after passing authentication</param>
        /// <returns>Redirect uri</returns>
        public virtual string BuildRedirectUri(string callbackUri = null)
        {
            if ( String.IsNullOrEmpty(callbackUri) ) callbackUri = Options.CallbackUri;

            // Setup required params
            string timestamp = DateTime.UtcNow.ToString("yyyy.MM.dd HH:mm:ss +0000");
            string state = Options.State.ToString("D");
            // Create signature in PKCS#7 detached signature UTF-8
            string clientSecret = BuildClientSecret(Options.Scope, timestamp, Options.ClientId, state);

            var builder = new StringBuilder();

            // Redirect uri to ESIA
            builder.Append(Options.RedirectUri);
            builder.AppendFormat("?client_id={0}", Uri.EscapeDataString(Options.ClientId)); // Client system identifier
            builder.AppendFormat("&scope={0}", Uri.EscapeDataString(Options.Scope));    // Scope
            builder.AppendFormat("&response_type={0}", Uri.EscapeDataString(Options.RequestType));  // Request type
            builder.AppendFormat("&state={0}", Uri.EscapeDataString(state));    // State
            builder.AppendFormat("&timestamp={0}", Uri.EscapeDataString(timestamp));    // Timestamp
            builder.AppendFormat("&access_type={0}", Uri.EscapeDataString(Options.AccessType == AccessType.Online ? "online" : "offline")); // Access type
            builder.AppendFormat("&redirect_uri={0}", Uri.EscapeDataString(callbackUri));   // Callback uri
            builder.AppendFormat("&client_secret={0}", Uri.EscapeDataString(clientSecret)); // Signature

            return builder.ToString();
        }

        /// <summary>
        /// Returns response for access token by authorization code
        /// </summary>
        /// <param name="authCode">Authorization code</param>
        /// <param name="callbackUri">Callback uri for ESIA. Must be equal to BuildRedirectUri call parameter</param>
        /// <returns>Token response</returns>
        public virtual async Task<EsiaTokenResponse> GetOAuthTokenAsync(string authCode, string callbackUri = null)
        {
            if ( String.IsNullOrEmpty(authCode) ) throw new ArgumentNullException("authCode");

            if ( String.IsNullOrEmpty(callbackUri) ) callbackUri = Options.CallbackUri;

            return await InternalGetOAuthTokenAsync(authCode, callbackUri, TokenRequest.ByAuthCode);
        }

        /// <summary>
        /// Returns response for access token by refresh token
        /// </summary>
        /// <param name="refreshToken">Refresh token</param>
        /// <param name="callbackUri">Callback uri for ESIA. Must be equal to BuildRedirectUri call parameter</param>
        /// <returns>Token response</returns>
        public virtual async Task<EsiaTokenResponse> GetOAuthTokenByRefreshAsync(string refreshToken, string callbackUri = null)
        {
            if ( String.IsNullOrEmpty(refreshToken) ) throw new ArgumentNullException("refreshToken");

            if ( String.IsNullOrEmpty(callbackUri) ) callbackUri = Options.CallbackUri;

            return await InternalGetOAuthTokenAsync(refreshToken, callbackUri, TokenRequest.ByRefresh);
        }

        /// <summary>
        /// Returns response for access token by client credential
        /// </summary>
        /// <param name="callbackUri">Callback uri for ESIA. Must be equal to BuildRedirectUri call parameter</param>
        /// <returns>Token response</returns>
        public virtual async Task<EsiaTokenResponse> GetOAuthTokenByCredentialsAsync(string callbackUri = null)
        {
            if ( String.IsNullOrEmpty(callbackUri) ) callbackUri = Options.CallbackUri;

            return await InternalGetOAuthTokenAsync(String.Empty, callbackUri, TokenRequest.ByCredential);
        }

        /// <summary>
        /// Returns response for access token
        /// </summary>
        /// <param name="code">Code, depends on request parameter</param>
        /// <param name="callbackUri">Callback uri for ESIA</param>
        /// <param name="request">Token request type</param>
        /// <returns>Token response</returns>
        protected virtual async Task<EsiaTokenResponse> InternalGetOAuthTokenAsync(string code, string callbackUri, TokenRequest request)
        {   // Setup required params
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

            // Build request content with params
            var requestContent = new FormUrlEncodedContent(requestParams);
#if DEBUG
            var content = await requestContent.ReadAsStringAsync();

            Debug.Write(content);
#endif
            // Send POST request to ESIA token uri
            HttpResponseMessage response = await HttpClient.PostAsync(Options.TokenUri, requestContent);

            // Update last status
            await UpdateLastStatusCode(response);
            // and check response status. Must be OK
            response.EnsureSuccessStatusCode();

            // Get response content and parse JSON string content to object
            string oauthTokenResponse = await response.Content.ReadAsStringAsync();
            JObject oauth2Token = JObject.Parse(oauthTokenResponse);
            // Get access token
            string accessToken = oauth2Token["access_token"].Value<string>();

            if ( string.IsNullOrWhiteSpace(accessToken) )
            {
                throw new Exception("Access token was not found");
            }

            // Sended and received state must be equal
            var value = oauth2Token.Value<string>("state");

            if ( value == null || value != state )
            {
                throw new Exception("State parameter is missing or invalid");
            }

            // Return token response instance
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

        /// <summary>
        /// Verifies access token signature
        /// </summary>
        /// <param name="accessToken">Access token</param>
        /// <returns>True if valid; otherwise, false</returns>
        public virtual bool VerifyToken(string accessToken)
        {
            if ( String.IsNullOrEmpty(accessToken) ) throw new ArgumentNullException("accessToken");

            string[] parts = accessToken.Split('.');
            string header = Encoding.UTF8.GetString(Base64Decode(parts[0]));
            JObject headerObject = JObject.Parse(header);

            return parts.Length > 2 && VerifySignature(headerObject.Value<string>("alg"), String.Format("{0}.{1}", parts[0], parts[1]), parts[2]);
        }

        /// <summary>
        /// Create EsiaToken instance from token response
        /// </summary>
        /// <param name="tokenResponse">Token response instanse</param>
        /// <returns>EsiaToken instanse</returns>
        public static EsiaToken CreateToken(EsiaTokenResponse tokenResponse)
        {
            if ( tokenResponse == null ) throw new ArgumentNullException("tokenResponse");

            string[] parts = tokenResponse.AccessToken.Split('.');
            string payload = Encoding.UTF8.GetString(Base64Decode(parts[1]));
            JObject payloadObject = JObject.Parse(payload);

            return new EsiaToken(tokenResponse.AccessToken, tokenResponse.RefreshToken, tokenResponse.ExpiresIn, payloadObject);
        }

        /// <summary>
        /// Sends request to ESIA
        /// </summary>
        /// <param name="method">Http method</param>
        /// <param name="requestUri">Request uri</param>
        /// <param name="styles">Flags. See SendStyles enumeration</param>
        /// <param name="requestParams">Parameters</param>
        /// <param name="headers">Additional headers</param>
        /// <returns>Http response</returns>
        public virtual async Task<HttpResponseMessage> SendAsync(HttpMethod method, string requestUri,
            SendStyles styles = SendStyles.Normal,
            IList<KeyValuePair<string, string>> requestParams = null, IList<KeyValuePair<string, string>> headers = null)
        {
            // Token must be set
            CheckTokenExist();

            // If SendStyles.CheckTokenTime flag is set, then check life time of token.
            // And if SendStyles.RefreshToken flag is set - update token by refresh token
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

            // Make request
            var response = await InternalSendAsync(method, requestUri, requestParams, headers);

            // If SendStyles.RefreshToken flag is set and response status is unauthorized - update token by refresh token
            // and repeat request
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

        /// <summary>
        /// Helper for GET request. Call SendAsync with http GET method
        /// </summary>
        /// <param name="requestUri">Request uri</param>
        /// <param name="styles">Flags. See SendStyles enumeration</param>
        /// <returns>Response string</returns>
        public virtual async Task<string> GetAsync(string requestUri, SendStyles styles = SendStyles.Normal)
        {
            var response = await SendAsync(HttpMethod.Get, requestUri, styles);

            if ( !response.IsSuccessStatusCode ) return null;

            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Helper for POST request. Call SendAsync with http POST method
        /// </summary>
        /// <param name="requestUri">Request uri</param>
        /// <param name="styles">Flags. See SendStyles enumeration</param>
        /// <param name="requestParams">Parameters</param>
        /// <returns>Response string</returns>
        public virtual async Task<string> PostAsync(string requestUri, SendStyles styles = SendStyles.Normal,
            IList<KeyValuePair<string, string>> requestParams = null)
        {
            var response = await SendAsync(HttpMethod.Post, requestUri, styles, requestParams);

            if ( !response.IsSuccessStatusCode ) return null;

            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Helper for PUT request. Call SendAsync with http PUT method
        /// </summary>
        /// <param name="requestUri">Request uri</param>
        /// <param name="styles">Flags. See SendStyles enumeration</param>
        /// <param name="requestParams">Parameters</param>
        /// <returns>Response string</returns>
        public virtual async Task<string> PutAsync(string requestUri, SendStyles styles = SendStyles.Normal,
            IList<KeyValuePair<string, string>> requestParams = null)
        {
            var response = await SendAsync(HttpMethod.Put, requestUri, styles, requestParams);

            if ( !response.IsSuccessStatusCode ) return null;

            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Update token by refresh token
        /// </summary>
        /// <param name="styles">Flags. See SendStyles enumeration</param>
        /// <returns></returns>
        private async Task UpdateToken(SendStyles styles)
        {
            var tokenResponse = await GetOAuthTokenByRefreshAsync(Token.RefreshToken);

            if ( styles.HasFlag(SendStyles.VerifyToken) && !VerifyToken(tokenResponse.AccessToken) ) throw new Exception("Token signature is invalid");

            Token = CreateToken(tokenResponse);
        }

        /// <summary>
        /// Make request to ESIA
        /// </summary>
        /// <param name="method">Http method</param>
        /// <param name="requestUri">Request uri</param>
        /// <param name="requestParams">Parameters</param>
        /// <param name="headers">Additional headers</param>
        /// <returns>Response</returns>
        protected virtual async Task<HttpResponseMessage> InternalSendAsync(HttpMethod method, string requestUri,
            IList<KeyValuePair<string, string>> requestParams, IList<KeyValuePair<string, string>> headers)
        {
            var message = new HttpRequestMessage(method, requestUri);

            // Set bearer authentication header
            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token.AccessToken);
            // Set media type to JSON
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Add additional headers
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

        /// <summary>
        /// Builds request signature
        /// </summary>
        /// <returns>Signature</returns>
        private string BuildClientSecret(string scope, string timestamp, string clientId, string state)
        {
            string signMessage = string.Format("{0}{1}{2}{3}", scope, timestamp, clientId, state);

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

        /// <summary>
        /// Verifies message signature
        /// </summary>
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