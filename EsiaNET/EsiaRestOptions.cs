namespace EsiaNET
{
    /// <summary>
    /// Provides options for ESIA
    /// </summary>
    public class EsiaRestOptions
    {
        /// <summary>
        /// Initializes a new instance of ESIA REST options
        /// </summary>
        public EsiaRestOptions()
        {
            RestUri = EsiaConsts.EsiaRestUrl;
            PrnsSfx = EsiaConsts.EsiaPrnsSfx;
            CttsSfx = EsiaConsts.EsiaCttsSfx;
            AddrsSfx = EsiaConsts.EsiaAddrsSfx;
            DocsSfx = EsiaConsts.EsiaDocsSfx;
            OrgsSfx = EsiaConsts.EsiaOrgsSfx;
            KidsSfx = EsiaConsts.EsiaKidsSfx;
            VhlsSfx = EsiaConsts.EsiaVhlsSfx;
        }

        /// <summary>
        /// ESIA REST url. Default: https://esia.gosuslugi.ru/rs
        /// </summary>
        public string RestUri { get; set; }

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
    }
}