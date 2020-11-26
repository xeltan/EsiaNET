using System;
using System.Collections.Generic;
using System.Net.Http;
using EsiaNET.AspNetCore.Authentication;

namespace EsiaNET.Test
{
    public class SignProvider : ISignProvider
    {
        public byte[] SignMessage(byte[] message)
        {
            var param = Convert.ToBase64String(message);
            var client = new HttpClient();
            var body = new HttpRequestMessage(HttpMethod.Get, "http://localhost:60569/sign?msg=" + Uri.EscapeDataString(param));

            var result = client.SendAsync(body).Result;

            result.EnsureSuccessStatusCode();

            var signature = result.Content.ReadAsStringAsync().Result;

            return Convert.FromBase64String(signature);
        }

        public bool VerifyMessage(string alg, byte[] message, byte[] signature)
        {
            var client = new HttpClient();
            var body = new HttpRequestMessage(HttpMethod.Post, "http://localhost:60569/verify");

            var verifyParameters = new Dictionary<string, string>()
            {
                { "Alg", alg },
                { "Message", Convert.ToBase64String(message) },
                { "Signature", Convert.ToBase64String(signature) }
            };

            var requestContent = new FormUrlEncodedContent(verifyParameters);

            body.Content = requestContent;

            var result = client.SendAsync(body).Result;

            if ( result.IsSuccessStatusCode )
            {
                return result.Content.ReadAsStringAsync().Result == "true";
            }

            return false;
        }
    }
}