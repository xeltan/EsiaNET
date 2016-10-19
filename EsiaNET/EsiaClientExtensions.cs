// EsiaNET version 0.1
// 
// Home page: 
// Author: https://github.com/xeltan

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace EsiaNET
{
    public static class EsiaClientExtensions
    {
        public static async Task<PersonInfo> GetPersonInfoAsync(this EsiaClient client, SendStyles styles = SendStyles.Normal)
        {
            if ( client.Token == null ) throw new ArgumentNullException("Token");

            return await client.GetPersonInfoAsync(client.Token.SbjId, styles);
        }

        public static async Task<PersonInfo> GetPersonInfoAsync(this EsiaClient client, string oid, SendStyles styles = SendStyles.Normal)
        {
            if ( String.IsNullOrEmpty(oid) ) throw new ArgumentNullException("oid");

            string uri = String.Format("{0}{1}", EsiaHelpers.NormalizeUri(client.Options.RestUri, client.Options.PrnsSfx), oid);
            var response = await client.GetAsync(uri, styles);

            return new PersonInfo(JObject.Parse(response));
        }

        public static async Task<IEnumerable<ContactInfo>> GetPersonContactsAsync(this EsiaClient client, SendStyles styles = SendStyles.Normal)
        {
            if ( client.Token == null ) throw new ArgumentNullException("Token");

            return await client.GetPersonContactsAsync(client.Token.SbjId, styles);
        }

        public static async Task<IEnumerable<ContactInfo>> GetPersonContactsAsync(this EsiaClient client, string oid, SendStyles styles = SendStyles.Normal)
        {
            if ( String.IsNullOrEmpty(oid) ) throw new ArgumentNullException("oid");

            string uri = String.Format("{0}{1}/{2}?embed=(elements)", EsiaHelpers.NormalizeUri(client.Options.RestUri, client.Options.PrnsSfx), oid,
                client.Options.CttsSfx);
            var response = await client.GetAsync(uri, styles);
            IDictionary<string, JToken> contactsDictionary = JObject.Parse(response);
            var result = new List<ContactInfo>();

            if ( contactsDictionary != null && contactsDictionary.ContainsKey("elements") )
            {
                foreach ( var contact in contactsDictionary["elements"] )
                {
                    var type = contact["type"].Value<string>();
                    ContactType contactType;

                    switch ( type )
                    {
                        case "MBT":
                            contactType = ContactType.Mobile;
                            break;
                        case "EML":
                            contactType = ContactType.Email;
                            break;
                        case "CEM":
                            contactType = ContactType.Cem;
                            break;
                        case "PHN":
                        default:
                            contactType = ContactType.Phone;
                            break;
                    }

                    result.Add(new ContactInfo(contactType, contact["value"].Value<string>(), contact["vrfStu"].Value<string>() == "VERIFIED"));
                }
            }

            return result;
        }

        public static async Task<IEnumerable<AddrInfo>> GetPersonAddrsAsync(this EsiaClient client, SendStyles styles = SendStyles.Normal)
        {
            if ( client.Token == null ) throw new ArgumentNullException("Token");

            return await client.GetPersonAddrsAsync(client.Token.SbjId, styles);
        }

        public static async Task<IEnumerable<AddrInfo>> GetPersonAddrsAsync(this EsiaClient client, string oid, SendStyles styles = SendStyles.Normal)
        {
            if ( String.IsNullOrEmpty(oid) ) throw new ArgumentNullException("oid");

            string uri = String.Format("{0}{1}/{2}?embed=(elements)", EsiaHelpers.NormalizeUri(client.Options.RestUri, client.Options.PrnsSfx), oid,
                client.Options.AddrsSfx);
            var response = await client.GetAsync(uri, styles);
            IDictionary<string, JToken> addrDictionary = JObject.Parse(response);
            var result = new List<AddrInfo>();

            if ( addrDictionary != null && addrDictionary.ContainsKey("elements") )
            {
                foreach ( var addr in addrDictionary["elements"] )
                {
                    var addrObject = addr as JObject;

                    if ( addrObject != null ) result.Add(new AddrInfo(addrObject));
                }
            }

            return result;
        }

        public static async Task<IEnumerable<DocInfo>> GetPersonDocsAsync(this EsiaClient client, SendStyles styles = SendStyles.Normal)
        {
            if ( client.Token == null ) throw new ArgumentNullException("Token");

            return await client.GetPersonDocsAsync(client.Token.SbjId, styles);
        }

        public static async Task<IEnumerable<DocInfo>> GetPersonDocsAsync(this EsiaClient client, string oid, SendStyles styles = SendStyles.Normal)
        {
            if ( String.IsNullOrEmpty(oid) ) throw new ArgumentNullException("oid");

            string uri = String.Format("{0}{1}/{2}?embed=(elements)", EsiaHelpers.NormalizeUri(client.Options.RestUri, client.Options.PrnsSfx), oid,
                client.Options.DocsSfx);
            var response = await client.GetAsync(uri, styles);
            IDictionary<string, JToken> docDictionary = JObject.Parse(response);
            var result = new List<DocInfo>();

            if ( docDictionary != null && docDictionary.ContainsKey("elements") )
            {
                foreach ( var doc in docDictionary["elements"] )
                {
                    var docObject = doc as JObject;

                    if ( docObject != null ) result.Add(new DocInfo(docObject));
                }
            }

            return result;
        }

        public static async Task<IEnumerable<PersonInfo>> GetPersonKidsAsync(this EsiaClient client, SendStyles styles = SendStyles.Normal)
        {
            if ( client.Token == null ) throw new ArgumentNullException("Token");

            return await client.GetPersonKidsAsync(client.Token.SbjId, styles);
        }

        public static async Task<IEnumerable<PersonInfo>> GetPersonKidsAsync(this EsiaClient client, string oid, SendStyles styles = SendStyles.Normal)
        {
            if ( String.IsNullOrEmpty(oid) ) throw new ArgumentNullException("oid");

            string uri = String.Format("{0}{1}/{2}?embed=(elements)", EsiaHelpers.NormalizeUri(client.Options.RestUri, client.Options.PrnsSfx), oid,
                client.Options.KidsSfx);
            var response = await client.GetAsync(uri, styles);
            IDictionary<string, JToken> kidsDictionary = JObject.Parse(response);
            var result = new List<PersonInfo>();

            if ( kidsDictionary != null && kidsDictionary.ContainsKey("elements") )
            {
                foreach ( var child in kidsDictionary["elements"] )
                {
                    var childObject = child as JObject;

                    if ( childObject != null ) result.Add(new PersonInfo(childObject));
                }
            }

            return result;
        }

        public static async Task<IEnumerable<DocInfo>> GetPersonChildDocsAsync(this EsiaClient client, string childOid, SendStyles styles = SendStyles.Normal)
        {
            if ( client.Token == null ) throw new ArgumentNullException("Token");

            return await client.GetPersonChildDocsAsync(client.Token.SbjId, childOid, styles);
        }

        public static async Task<IEnumerable<DocInfo>> GetPersonChildDocsAsync(this EsiaClient client, string oid, string childOid,
            SendStyles styles = SendStyles.Normal)
        {
            if ( String.IsNullOrEmpty(oid) ) throw new ArgumentNullException("oid");

            string uri = String.Format("{0}{1}/{2}/{3}/{4}?embed=(elements)", EsiaHelpers.NormalizeUri(client.Options.RestUri, client.Options.PrnsSfx), oid,
                client.Options.KidsSfx, childOid, client.Options.DocsSfx);
            var response = await client.GetAsync(uri, styles);
            IDictionary<string, JToken> docDictionary = JObject.Parse(response);
            var result = new List<DocInfo>();

            if ( docDictionary != null && docDictionary.ContainsKey("elements") )
            {
                foreach ( var doc in docDictionary["elements"] )
                {
                    var docObject = doc as JObject;

                    if ( docObject != null ) result.Add(new DocInfo(docObject));
                }
            }

            return result;
        }

        public static async Task<IEnumerable<VehicleInfo>> GetPersonVehiclesAsync(this EsiaClient client, SendStyles styles = SendStyles.Normal)
        {
            if ( client.Token == null ) throw new ArgumentNullException("Token");

            return await client.GetPersonVehiclesAsync(client.Token.SbjId, styles);
        }

        public static async Task<IEnumerable<VehicleInfo>> GetPersonVehiclesAsync(this EsiaClient client, string oid, SendStyles styles = SendStyles.Normal)
        {
            if ( String.IsNullOrEmpty(oid) ) throw new ArgumentNullException("oid");

            string uri = String.Format("{0}{1}/{2}?embed=(elements)", EsiaHelpers.NormalizeUri(client.Options.RestUri, client.Options.PrnsSfx), oid,
                client.Options.VhlsSfx);
            var response = await client.GetAsync(uri, styles);
            IDictionary<string, JToken> vehicleDictionary = JObject.Parse(response);
            var result = new List<VehicleInfo>();

            if ( vehicleDictionary != null && vehicleDictionary.ContainsKey("elements") )
            {
                foreach ( var vehicle in vehicleDictionary["elements"] )
                {
                    var vehicleObject = vehicle as JObject;

                    if ( vehicleObject != null ) result.Add(new VehicleInfo(vehicleObject));
                }
            }

            return result;
        }
    }
}