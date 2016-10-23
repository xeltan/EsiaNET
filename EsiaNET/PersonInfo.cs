// EsiaNET version 0.1
// 
// Home page: 
// Author: https://github.com/xeltan

using System;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace EsiaNET
{
    /// <summary>
    /// Provides owner personal information
    /// </summary>
    public class PersonInfo
    {
        internal PersonInfo(JObject personInfo)
        {
            if ( personInfo != null )
            {
                Id = EsiaHelpers.PropertyValueIfExists("id", personInfo);
                FirstName = EsiaHelpers.PropertyValueIfExists("firstName", personInfo);
                LastName = EsiaHelpers.PropertyValueIfExists("lastName", personInfo);
                MiddleName = EsiaHelpers.PropertyValueIfExists("middleName", personInfo);
                BirthPlace = EsiaHelpers.PropertyValueIfExists("birthPlace", personInfo);
                Gender = EsiaHelpers.PropertyValueIfExists("gender", personInfo);
                Citizenship = EsiaHelpers.PropertyValueIfExists("citizenship", personInfo);
                Snils = EsiaHelpers.PropertyValueIfExists("snils", personInfo);
                Inn = EsiaHelpers.PropertyValueIfExists("inn", personInfo);

                var value = EsiaHelpers.PropertyValueIfExists("trusted", personInfo);

                Trusted = !String.IsNullOrWhiteSpace(value) && value.ToLowerInvariant() == "true";

                if ( !String.IsNullOrWhiteSpace(LastName) )
                {
                    Name = LastName;

                    if ( !String.IsNullOrWhiteSpace(FirstName) ) Name = String.Format("{0} {1}", Name, FirstName);
                    if ( !String.IsNullOrWhiteSpace(MiddleName) ) Name = String.Format("{0} {1}", Name, MiddleName);
                }

                value = EsiaHelpers.PropertyValueIfExists("birthDate", personInfo);

                DateTime date;

                if ( DateTime.TryParse(value, new CultureInfo("ru-RU"), DateTimeStyles.AssumeLocal, out date) )
                {
                    BirthDate = date;
                }
            }
        }

        public string Id { get; }

        /// <summary>
        /// User name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// User first name
        /// </summary>
        public string FirstName { get; }

        /// <summary>
        /// User last name
        /// </summary>
        public string LastName { get; }

        /// <summary>
        /// User middle name
        /// </summary>
        public string MiddleName { get; }

        /// <summary>
        /// User birthdate
        /// </summary>
        public DateTime? BirthDate { get; }

        /// <summary>
        /// User birth place
        /// </summary>
        public string BirthPlace { get; }

        /// <summary>
        /// User Gender
        /// </summary>
        public string Gender { get; }

        /// <summary>
        /// True if owner is trusted; otherwise, false
        /// </summary>
        public bool Trusted { get; }

        /// <summary>
        /// Citizenship
        /// </summary>
        public string Citizenship { get; }

        /// <summary>
        /// SNILS
        /// </summary>
        public string Snils { get; }

        /// <summary>
        /// INN
        /// </summary>
        public string Inn { get; }
    }
}