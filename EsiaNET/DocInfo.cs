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

    /// <summary>
    /// Provides owner document information
    /// </summary>
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

        /// <summary>
        /// Document type
        /// </summary>
        public DocType DocType { get; }

        /// <summary>
        /// True if document was verified by government org; otherwise, false
        /// </summary>
        public bool Verified { get; }

        /// <summary>
        /// Document seria
        /// </summary>
        public string Series { get; }

        /// <summary>
        /// Document number
        /// </summary>
        public string Number { get; }

        /// <summary>
        /// Issue date (дата выдачи)
        /// </summary>
        public string IssueDate { get; }

        /// <summary>
        /// Issue code (код подразделения)
        /// </summary>
        public string IssueId { get; }

        /// <summary>
        /// Issued by (кем выдан)
        /// </summary>
        public string IssuedBy { get; }

        /// <summary>
        /// Expiration date (срок действия документа)
        /// </summary>
        public string ExpiryDate { get; }

        /// <summary>
        /// First name (имя для заграничного паспорта)
        /// </summary>
        public string FirstName { get; }

        /// <summary>
        /// Last name (фамилия для заграничного паспорта)
        /// </summary>
        public string LastName { get; }
    }
}