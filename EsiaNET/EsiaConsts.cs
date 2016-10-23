// EsiaNET version 0.1
// 
// Home page: 
// Author: https://github.com/xeltan

namespace EsiaNET
{
    /// <summary>
    /// ESIA constants
    /// </summary>
    public class EsiaConsts
    {
        /// <summary>
        /// ESIA authorization url
        /// </summary>
        public const string EsiaAuthUrl = "https://esia.gosuslugi.ru/aas/oauth2/ac";

        /// <summary>
        /// ESIA access token url
        /// </summary>
        public const string EsiaTokenUrl = "https://esia.gosuslugi.ru/aas/oauth2/te";

        /// <summary>
        /// ESIA REST services url
        /// </summary>
        public const string EsiaRestUrl = "https://esia.gosuslugi.ru/rs";

        /// <summary>
        /// ESIA test authorization url
        /// </summary>
        public const string EsiaAuthTestUrl = "https://esia-portal1.test.gosuslugi.ru/aas/oauth2/ac";

        /// <summary>
        /// ESIA test access token url
        /// </summary>
        public const string EsiaTokenTestUrl = "https://esia-portal1.test.gosuslugi.ru/aas/oauth2/te";

        /// <summary>
        /// ESIA test REST services url
        /// </summary>
        public const string EsiaRestTestUrl = "https://esia-portal1.test.gosuslugi.ru/rs";

        /// <summary>
        /// Suffix of REST person API
        /// </summary>
        public const string EsiaPrnsSfx = "prns";

        /// <summary>
        /// Suffix of REST contacts API
        /// </summary>
        public const string EsiaCttsSfx = "ctts";

        /// <summary>
        /// Suffix of REST addresses API
        /// </summary>
        public const string EsiaAddrsSfx = "addrs";

        /// <summary>
        /// Suffix of REST documents API
        /// </summary>
        public const string EsiaDocsSfx = "docs";

        /// <summary>
        /// Suffix of REST orgs API
        /// </summary>
        public const string EsiaOrgsSfx = "orgs";

        /// <summary>
        /// Suffix of REST kids API
        /// </summary>
        public const string EsiaKidsSfx = "kids";

        /// <summary>
        /// Suffix of REST vehicles API
        /// </summary>
        public const string EsiaVhlsSfx = "vhls";
    }
}