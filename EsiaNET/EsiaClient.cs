using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace EsiaNET
{
    /// <summary>
    /// Provides a base class for communication with ESIA REST services
    /// </summary>
    public class EsiaClient
    {
        private HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of EsiaClient class with options 
        /// </summary>
        /// <param name="options">ESIA options class instance</param>
        /// <param name="httpClient">Backchannel http client. Default is null - will be created automatically.</param>
        public EsiaClient(EsiaRestOptions options, HttpClient httpClient = null)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));

            _httpClient = httpClient;
        }

        /// <summary>
        /// Initializes a new instance of EsiaClient class with options and access token
        /// </summary>
        /// <param name="options">ESIA options class instance</param>
        /// <param name="accessToken">Access token</param>
        /// <param name="httpClient">Backchannel http client. Default is null - will be created automatically.</param>
        public EsiaClient(EsiaRestOptions options, string accessToken, HttpClient httpClient = null) : this(options,
            new EsiaToken(accessToken), httpClient)
        {
        }

        /// <summary>
        /// Initializes a new instance of EsiaClient class with options and access token
        /// </summary>
        /// <param name="options">ESIA options class instance</param>
        /// <param name="token">Instance of EsiaToken class</param>
        /// <param name="httpClient">Backchannel http client. Default is null - will be created automatically.</param>
        public EsiaClient(EsiaRestOptions options, EsiaToken token, HttpClient httpClient = null) : this(options, httpClient)
        {
            Token = token ?? throw new ArgumentNullException(nameof(token));
        }

        /// <summary>
        /// ESIA Options
        /// </summary>
        public EsiaRestOptions Options { get; }

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
        /// Sends request to ESIA
        /// </summary>
        /// <param name="method">Http method</param>
        /// <param name="requestUri">Request uri</param>
        /// <param name="requestParams">Parameters</param>
        /// <param name="headers">Additional headers</param>
        /// <returns>Http response</returns>
        public virtual async Task<HttpResponseMessage> SendAsync(HttpMethod method, string requestUri,
            IList<KeyValuePair<string, string>> requestParams = null, IList<KeyValuePair<string, string>> headers = null)
        {
            // Token must be set
            CheckTokenExist();

            // Make request
            var response = await InternalSendAsync(method, requestUri, requestParams, headers);

            return response;
        }

        /// <summary>
        /// Helper for GET request. Call SendAsync with http GET method
        /// </summary>
        /// <param name="requestUri">Request uri</param>
        /// <returns>Response string</returns>
        public virtual async Task<string> GetAsync(string requestUri)
        {
            var response = await SendAsync(HttpMethod.Get, requestUri);

            if ( !response.IsSuccessStatusCode ) return null;

            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Helper for POST request. Call SendAsync with http POST method
        /// </summary>
        /// <param name="requestUri">Request uri</param>
        /// <param name="requestParams">Parameters</param>
        /// <returns>Response string</returns>
        public virtual async Task<string> PostAsync(string requestUri,
            IList<KeyValuePair<string, string>> requestParams = null)
        {
            var response = await SendAsync(HttpMethod.Post, requestUri, requestParams);

            if ( !response.IsSuccessStatusCode ) return null;

            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Helper for PUT request. Call SendAsync with http PUT method
        /// </summary>
        /// <param name="requestUri">Request uri</param>
        /// <param name="requestParams">Parameters</param>
        /// <returns>Response string</returns>
        public virtual async Task<string> PutAsync(string requestUri,
            IList<KeyValuePair<string, string>> requestParams = null)
        {
            var response = await SendAsync(HttpMethod.Put, requestUri, requestParams);

            if ( !response.IsSuccessStatusCode ) return null;

            return await response.Content.ReadAsStringAsync();
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

        private async Task UpdateLastStatusCode(HttpResponseMessage response)
        {
            LastStatusCode = response.StatusCode;
            LastStatusMessage = null;

            if ( !response.IsSuccessStatusCode )
            {
                LastStatusMessage = await response.Content.ReadAsStringAsync();
            }
        }

        private void CheckTokenExist()
        {
            if ( Token == null ) throw new ArgumentNullException("Token");
        }

        private HttpClient CreateHttpClient()
        {
            return new HttpClient
            {
                MaxResponseContentBufferSize = 1024 * 1024 * 10 // 10 MB
            };
        }
    }
}