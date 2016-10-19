// EsiaNET version 0.1
// 
// Home page: 
// Author: https://github.com/xeltan

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace EsiaNET
{
    public static class EsiaHelpers
    {
        public static string PropertyValueIfExists(string property, IDictionary<string, JToken> dictionary)
        {
            return dictionary.ContainsKey(property) ? dictionary[property].ToString() : null;
        }

        public static DateTime DateFromUnixSeconds(double seconds)
        {
            var date = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            return date.AddSeconds(seconds).ToLocalTime();
        }

        public static string NormalizeUri(params string[] uriParts)
        {
            var builder = new StringBuilder();

            foreach ( var part in uriParts )
            {
                builder.Append(part);

                if ( part.Last() != '/' && part.Last() != '\\' ) builder.Append("/");
            }

            return builder.ToString();
        }
    }
}