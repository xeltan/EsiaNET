// EsiaNET version 0.1
// 
// Home page: 
// Author: https://github.com/xeltan

using Newtonsoft.Json.Linq;

namespace EsiaNET
{
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

        public string Name { get; }

        public string NumberPlate { get; }

        public string RegSeries { get; }

        public string RegNumber { get; }
    }
}