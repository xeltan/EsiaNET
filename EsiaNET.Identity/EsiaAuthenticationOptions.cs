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
    /// <summary>
    /// Options for ESIA authentication middleware
    /// </summary>
    public class EsiaAuthenticationOptions : AuthenticationOptions
    {
        /// <summary>
        /// Initialize a new instance with ESIA client options passed to EsiaClient instance
        /// </summary>
        /// <param name="options">Base ESIA options</param>
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

        /// <summary>
        /// ESIA client options
        /// </summary>
        public EsiaOptions EsiaOptions { get; }

        /// <summary>
        /// Gets or sets the display name for the ESIA authentication provider
        /// </summary>
        public string Caption
        {
            get { return Description.Caption; }
            set { Description.Caption = value; }
        }

        /// <summary>
        /// Gets or sets the type used to secure data handled by the middleware.
        /// </summary>
        public ISecureDataFormat<AuthenticationProperties> DataFormat { get; set; }

        /// <summary>
        /// Gets or sets provider with callbacks
        /// </summary>
        public IEsiaAuthenticationProvider Provider { get; set; }

        /// <summary>
        /// True if middleware needs to verify token signature; otherwise, false
        /// </summary>
        public bool VerifyTokenSignature { get; set; }

        /// <summary>
        /// Gets or sets the name of another authentication middleware which will be responsible for actually issuing a user <see cref="System.Security.Claims.ClaimsIdentity"/>.
        /// </summary>
        public string SignInAsAuthenticationType { get; set; }

        public bool GetUserInfo { get; set; }
    }
}