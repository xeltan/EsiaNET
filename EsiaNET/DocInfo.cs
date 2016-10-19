// EsiaNET version 0.1
// 
// Home page: 
// Author: https://github.com/xeltan

using Newtonsoft.Json.Linq;

namespace EsiaNET
{
    public enum DocType
    {
        None,
        Passport, // RF_PASSPORT - паспорт РФ
        Foreign, // FID_DOC - документ иностранного гражданина
        DrivingLicense, // RF_DRIVING_LICENSE - водительское удостоверение
        Military, // MLTR_ID - военный билет
        ForeignPassport, // FRGN_PASS - заграничный паспорт
        MedicalPolicy, // MDCL_PLCY - полис ОМС
        BirthCert // BRTH_CERT - свидетельство о рождении
    }

    public class DocInfo
    {
        public DocInfo(JObject docInfo)
        {
            DocType = DocType.None;

            if ( docInfo != null )
            {
                var value = EsiaHelpers.PropertyValueIfExists("type", docInfo);

                switch ( value )
                {
                    case "RF_PASSPORT":
                        DocType = DocType.Passport;
                        break;
                    case "FID_DOC":
                        DocType = DocType.Foreign;
                        break;
                    case "RF_DRIVING_LICENSE":
                        DocType = DocType.DrivingLicense;
                        break;
                    case "MLTR_ID":
                        DocType = DocType.Military;
                        break;
                    case "FRGN_PASS":
                        DocType = DocType.ForeignPassport;
                        break;
                    case "MDCL_PLCY":
                        DocType = DocType.MedicalPolicy;
                        break;
                    case "BRTH_CERT":
                        DocType = DocType.BirthCert;
                        break;
                    default:
                        DocType = DocType.None;
                        break;
                }

                Verified = docInfo["vrfStu"].Value<string>() == "VERIFIED";

                Series = EsiaHelpers.PropertyValueIfExists("series", docInfo);
                Number = EsiaHelpers.PropertyValueIfExists("number", docInfo);
                IssueDate = EsiaHelpers.PropertyValueIfExists("issueDate", docInfo);
                IssueId = EsiaHelpers.PropertyValueIfExists("issueId", docInfo);
                IssuedBy = EsiaHelpers.PropertyValueIfExists("issuedBy", docInfo);
                ExpiryDate = EsiaHelpers.PropertyValueIfExists("expiryDate", docInfo);
                FirstName = EsiaHelpers.PropertyValueIfExists("firstName", docInfo);
                LastName = EsiaHelpers.PropertyValueIfExists("lastName", docInfo);
            }
        }

        public DocType DocType { get; }

        public bool Verified { get; }

        public string Series { get; }

        public string Number { get; }

        public string IssueDate { get; }

        public string IssueId { get; }

        public string IssuedBy { get; }

        public string ExpiryDate { get; }

        public string FirstName { get; }

        public string LastName { get; }
    }
}