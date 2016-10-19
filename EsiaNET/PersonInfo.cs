// EsiaNET version 0.1
// 
// Home page: 
// Author: https://github.com/xeltan

using System;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace EsiaNET
{
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
        /// Gets the user name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the user first name
        /// </summary>
        public string FirstName { get; private set; }

        /// <summary>
        /// Gets the user last name
        /// </summary>
        public string LastName { get; private set; }

        /// <summary>
        /// Gets the user middle name
        /// </summary>
        public string MiddleName { get; private set; }

        public DateTime? BirthDate { get; private set; }

        public string BirthPlace { get; private set; }

        public string Gender { get; private set; }

        public bool Trusted { get; private set; }

        public string Citizenship { get; private set; }

        public string Snils { get; private set; }

        public string Inn { get; private set; }
    }
}