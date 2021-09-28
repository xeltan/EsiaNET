using System;
using Newtonsoft.Json.Linq;

namespace EsiaNET
{
    /// <summary>
    /// Provides personal role information
    /// </summary>
    public class PersonRole
    {
        internal PersonRole(JObject personRole)
        {
            if (personRole == null)
            {
                return;
            }
            
            Oid = Convert.ToInt64(EsiaHelpers.PropertyValueIfExists("oid", personRole));
            PersonOid = Convert.ToInt64(EsiaHelpers.PropertyValueIfExists("prnOid", personRole));
            FullName = EsiaHelpers.PropertyValueIfExists("fullName", personRole);
            ShortName = EsiaHelpers.PropertyValueIfExists("shortName", personRole);
            Ogrn = EsiaHelpers.PropertyValueIfExists("ogrn", personRole);
            Type = EsiaHelpers.PropertyValueIfExists("type", personRole);
            Chief = EsiaHelpers.PropertyBoolValueIfExists("chief", personRole);
            Admin = EsiaHelpers.PropertyBoolValueIfExists("admin", personRole);
            Phone = EsiaHelpers.PropertyValueIfExists("phone", personRole);
            Email = EsiaHelpers.PropertyValueIfExists("email", personRole);
            Active = EsiaHelpers.PropertyBoolValueIfExists("active", personRole);
            HasRightOfSubstitution = EsiaHelpers.PropertyBoolValueIfExists("hasRightOfSubstitution", personRole);
            HasApprovalTabAccess = EsiaHelpers.PropertyBoolValueIfExists("hasApprovalTabAccess", personRole);
            IsLiquidated = EsiaHelpers.PropertyBoolValueIfExists("isLiquidated", personRole);
        }

        /// <summary>
        /// Organization identifier
        /// </summary>
        public long Oid { get; set; }

        /// <summary>
        /// Person Organization identifier
        /// </summary>
        public long PersonOid { get; set; }

        /// <summary>
        /// Full name for organization
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Short name for organization
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// Ogrn
        /// </summary>
        public string Ogrn { get; set; }

        /// <summary>
        /// Type for organization
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Chief current organization
        /// </summary>
        public bool Chief { get; set; }

        /// <summary>
        /// Administrator for current organization
        /// </summary>
        public bool Admin { get; set; }

        /// <summary>
        /// Contact phone number
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Contact Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Flag determinate what a company have active status
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Flag determinate what a company has right of substitution
        /// </summary>
        public bool? HasRightOfSubstitution { get; set; }

        /// <summary>
        /// Flag determinate what a company has approval tab access
        /// </summary>
        public bool HasApprovalTabAccess { get; set; }

        /// <summary>
        /// Flag determinate what a company is liquidated
        /// </summary>
        public bool IsLiquidated { get; set; }
    }
}