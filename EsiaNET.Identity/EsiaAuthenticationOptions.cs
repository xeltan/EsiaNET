// EsiaNET.Identity version 0.1
// 
// Home page: 
// Author: https://github.com/xeltan

using System;
using EsiaNET.Identity.Provider;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace EsiaNET.Identity
{
    public static class RequestTypes
    {
        public const string Code = "code";
    }

    public class EsiaAuthenticationOptions : AuthenticationOptions
    {
        public EsiaAuthenticationOptions(EsiaOptions options) : base("ESIA")
        {
            if ( options == null ) throw new ArgumentNullException("options");

            EsiaOptions = options;

            if ( String.IsNullOrEmpty(EsiaOptions.CallbackUri) ) EsiaOptions.CallbackUri = new PathString("/esia-signin").ToString();

            AuthenticationMode = AuthenticationMode.Passive;
            Caption = "ЕСИА";
            VerifyTokenSignature = true;
            GetUserInfo = true;
        }

        public EsiaOptions EsiaOptions { get; }

        public string Caption
        {
            get { return Description.Caption; }
            set { Description.Caption = value; }
        }

        /// <summary>
        /// Gets or sets the type used to secure data handled by the middleware.
        /// </summary>
        public ISecureDataFormat<AuthenticationProperties> DataFormat { get; set; }

        public IEsiaAuthenticationProvider Provider { get; set; }

        public bool VerifyTokenSignature { get; set; }

        /// <summary>
        /// Gets or sets the name of another authentication middleware which will be responsible for actually issuing a user <see cref="System.Security.Claims.ClaimsIdentity"/>.
        /// </summary>
        public string SignInAsAuthenticationType { get; set; }

        public bool GetUserInfo { get; set; }
    }
}