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

    public class EsiaOptions
    {
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

        public string RedirectUri { get; set; }

        public string TokenUri { get; set; }

        public string RestUri { get; set; }

        public string CallbackUri { get; set; }

        public TimeSpan BackchannelTimeout { get; set; }

        public string ClientId { get; set; }

        public string Scope { get; set; }

        public Guid State { get; set; }

        public AccessType AccessType { get; set; }

        public string RequestType { get; set; }

        public ISignProvider SignProvider { get; set; }

        public string PrnsSfx { get; set; }

        public string CttsSfx { get; set; }

        public string AddrsSfx { get; set; }

        public string DocsSfx { get; set; }

        public string OrgsSfx { get; set; }

        public string KidsSfx { get; set; }

        public string VhlsSfx { get; set; }

        public static ISignProvider CreateSignProvider(Func<X509Certificate2> action, Func<X509Certificate2> esiaAction = null)
        {
            if ( action == null ) throw new ArgumentException("Get certificate action must be provided");

            return new DefaultSignProvider(action, esiaAction);
        }
    }
}