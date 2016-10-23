// EsiaNET version 0.1
// 
// Home page: 
// Author: https://github.com/xeltan

using System;
using System.Security.Cryptography.X509Certificates;

namespace EsiaNET
{
    public static class RequestTypes
    {
        public const string Code = "code";
    }

    /// <summary>
    /// Provides options for ESIA
    /// </summary>
    public class EsiaOptions
    {
        /// <summary>
        /// Initializes a new instanse of ESIA options
        /// </summary>
        public EsiaOptions()
        {
            BackchannelTimeout = TimeSpan.FromSeconds(60);
            State = Guid.NewGuid();
            RedirectUri = EsiaConsts.EsiaAuthUrl;
            TokenUri = EsiaConsts.EsiaTokenUrl;
            RestUri = EsiaConsts.EsiaRestUrl;
            AccessType = AccessType.Online;
            RequestType = RequestTypes.Code;
            PrnsSfx = EsiaConsts.EsiaPrnsSfx;
            CttsSfx = EsiaConsts.EsiaCttsSfx;
            AddrsSfx = EsiaConsts.EsiaAddrsSfx;
            DocsSfx = EsiaConsts.EsiaDocsSfx;
            OrgsSfx = EsiaConsts.EsiaOrgsSfx;
            KidsSfx = EsiaConsts.EsiaKidsSfx;
            VhlsSfx = EsiaConsts.EsiaVhlsSfx;
        }

        /// <summary>
        /// ESIA authentication url to redirect. Default: https://esia.gosuslugi.ru/aas/oauth2/ac
        /// </summary>
        public string RedirectUri { get; set; }

        /// <summary>
        /// ESIA access token url. Default: https://esia.gosuslugi.ru/aas/oauth2/te
        /// </summary>
        public string TokenUri { get; set; }

        /// <summary>
        /// ESIA REST url. Default: https://esia.gosuslugi.ru/rs
        /// </summary>
        public string RestUri { get; set; }

        /// <summary>
        /// Callback url for redirecting from ESIA after authentication
        /// </summary>
        public string CallbackUri { get; set; }

        /// <summary>
        /// Http operation timeout. Default: 60 sec
        /// </summary>
        public TimeSpan BackchannelTimeout { get; set; }

        /// <summary>
        /// Client system identifier. Required.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Scope for client system. Required
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// State parameter. Default is new Guid
        /// </summary>
        public Guid State { get; set; }

        /// <summary>
        /// Access type parameter. Default: AccessType.Online
        /// </summary>
        public AccessType AccessType { get; set; }

        /// <summary>
        /// Request type parameter. Drfault: code
        /// </summary>
        public string RequestType { get; set; }

        /// <summary>
        /// Sign provider to get client system and ESIA certificates. Required
        /// </summary>
        public ISignProvider SignProvider { get; set; }

        /// <summary>
        /// Suffix of REST person API. Default: prns
        /// </summary>
        public string PrnsSfx { get; set; }

        /// <summary>
        /// Suffix of REST contacts API. Default: ctts
        /// </summary>
        public string CttsSfx { get; set; }

        /// <summary>
        /// Suffix of REST addresses API. Default: addrs
        /// </summary>
        public string AddrsSfx { get; set; }

        /// <summary>
        /// Suffix of REST documents API. Defaults: docs
        /// </summary>
        public string DocsSfx { get; set; }

        /// <summary>
        /// Suffix of REST orgs API. Default: orgs
        /// </summary>
        public string OrgsSfx { get; set; }

        /// <summary>
        /// Suffix of REST kids API. Default: kids
        /// </summary>
        public string KidsSfx { get; set; }

        /// <summary>
        /// Suffix of REST vehicles API. Default: vhls
        /// </summary>
        public string VhlsSfx { get; set; }

        /// <summary>
        /// Creates default sign provider for custom handlers
        /// </summary>
        /// <param name="action">Handler that returns client system certificate registered with ESIA</param>
        /// <param name="esiaAction">Handler that returns ESIA certificate. Used only with token verification</param>
        /// <returns>Default sign provider with handlers</returns>
        public static ISignProvider CreateSignProvider(Func<X509Certificate2> action, Func<X509Certificate2> esiaAction = null)
        {
            if ( action == null ) throw new ArgumentException("Get certificate action must be provided");

            return new DefaultSignProvider(action, esiaAction);
        }
    }
}