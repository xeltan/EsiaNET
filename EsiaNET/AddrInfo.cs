// EsiaNET version 0.1
// 
// Home page: 
// Author: https://github.com/xeltan

using Newtonsoft.Json.Linq;

namespace EsiaNET
{
    public enum AddrType
    {
        Residential, // PLV - адрес проживания
        Registration // PRG - адрес регистрации
    }

    /// <summary>
    /// Provides owner address information
    /// </summary>
    public class AddrInfo
    {
        public AddrInfo(JObject addrInfo)
        {
            if ( addrInfo != null )
            {
                var value = EsiaHelpers.PropertyValueIfExists("type", addrInfo);

                switch ( value )
                {
                    case "PLV":
                        AddrType = AddrType.Residential;
                        break;
                    default:
                        AddrType = AddrType.Registration;
                        break;
                }

                ZipCode = EsiaHelpers.PropertyValueIfExists("zipCode", addrInfo);
                CountryId = EsiaHelpers.PropertyValueIfExists("countryId", addrInfo);
                AddressStr = EsiaHelpers.PropertyValueIfExists("addressStr", addrInfo);
                Building = EsiaHelpers.PropertyValueIfExists("building", addrInfo);
                Frame = EsiaHelpers.PropertyValueIfExists("frame", addrInfo);
                House = EsiaHelpers.PropertyValueIfExists("house", addrInfo);
                Flat = EsiaHelpers.PropertyValueIfExists("flat", addrInfo);
                FiasCode = EsiaHelpers.PropertyValueIfExists("fiasCode", addrInfo);
                Region = EsiaHelpers.PropertyValueIfExists("region", addrInfo);
                City = EsiaHelpers.PropertyValueIfExists("city", addrInfo);
                District = EsiaHelpers.PropertyValueIfExists("district", addrInfo);
                Area = EsiaHelpers.PropertyValueIfExists("area", addrInfo);
                Settlement = EsiaHelpers.PropertyValueIfExists("settlement", addrInfo);
                AdditionArea = EsiaHelpers.PropertyValueIfExists("additionArea", addrInfo);
                AdditionAreaStreet = EsiaHelpers.PropertyValueIfExists("additionAreaStreet", addrInfo);
                Street = EsiaHelpers.PropertyValueIfExists("street", addrInfo);
            }
        }

        /// <summary>
        /// Address type
        /// </summary>
        public AddrType AddrType { get; }

        /// <summary>
        /// Postal index
        /// </summary>
        public string ZipCode { get; }

        /// <summary>
        /// Country short code
        /// </summary>
        public string CountryId { get; }

        /// <summary>
        /// Address string without house and flat
        /// </summary>
        public string AddressStr { get; }

        public string Building { get; }

        public string Frame { get; }

        public string House { get; }

        public string Flat { get; }

        public string FiasCode { get; }

        public string Region { get; }

        public string City { get; }

        public string District { get; }

        public string Area { get; }

        public string Settlement { get; }

        public string AdditionArea { get; }

        public string AdditionAreaStreet { get; }

        public string Street { get; }
    }
}