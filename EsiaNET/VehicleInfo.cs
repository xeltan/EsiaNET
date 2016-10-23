// EsiaNET version 0.1
// 
// Home page: 
// Author: https://github.com/xeltan

using Newtonsoft.Json.Linq;

namespace EsiaNET
{
    /// <summary>
    /// Provides owner vehicles
    /// </summary>
    public class VehicleInfo
    {
        public VehicleInfo(JObject vehicleInfo)
        {
            if ( vehicleInfo != null )
            {
                Id = EsiaHelpers.PropertyValueIfExists("id", vehicleInfo);
                Name = EsiaHelpers.PropertyValueIfExists("name", vehicleInfo);
                NumberPlate = EsiaHelpers.PropertyValueIfExists("numberPlate", vehicleInfo);

                var regObject = vehicleInfo["regCertificate"] as JObject;

                if ( regObject != null )
                {
                    RegSeries = EsiaHelpers.PropertyValueIfExists("series", regObject);
                    RegNumber = EsiaHelpers.PropertyValueIfExists("number", regObject);
                }
            }
        }

        public string Id { get; }

        /// <summary>
        /// Vehicle name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Vehicle registration plate
        /// </summary>
        public string NumberPlate { get; }

        /// <summary>
        /// Registration certificate seria (серия свидетельства о гос. регистрации)
        /// </summary>
        public string RegSeries { get; }

        /// <summary>
        /// Registration certificate number (номер свидетельства о гос. регистрации)
        /// </summary>
        public string RegNumber { get; }
    }
}