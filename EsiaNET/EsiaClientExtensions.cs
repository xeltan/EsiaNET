﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace EsiaNET
{
    public static class EsiaClientExtensions
    {
        /// <summary>
        /// Get personal information about authorized user
        /// </summary>
        /// <param name="client"></param>
        /// <returns>PersonInfo instance</returns>
        public static async Task<PersonInfo> GetPersonInfoAsync(this EsiaClient client)
        {
            if ( client.Token == null ) throw new ArgumentNullException("Token");

            return await client.GetPersonInfoAsync(client.Token.SbjId);
        }

        /// <summary>
        /// Get personal information about specified user
        /// </summary>
        /// <param name="client"></param>
        /// <param name="oid">User oid</param>
        /// <returns>PersonInfo instance</returns>
        public static async Task<PersonInfo> GetPersonInfoAsync(this EsiaClient client, string oid)
        {
            if ( String.IsNullOrEmpty(oid) ) throw new ArgumentNullException("oid");

            string uri = String.Format("{0}{1}", EsiaHelpers.NormalizeUri(client.Options.RestUri, client.Options.PrnsSfx), oid);
            var response = await client.GetAsync(uri);

            return response == null ? null : new PersonInfo(JObject.Parse(response));
        }

        /// <summary>
        /// Get contacts of authorized user
        /// </summary>
        /// <param name="client"></param>
        /// <returns>ContactInfo array</returns>
        public static async Task<IEnumerable<ContactInfo>> GetPersonContactsAsync(this EsiaClient client)
        {
            if ( client.Token == null ) throw new ArgumentNullException("Token");

            return await client.GetPersonContactsAsync(client.Token.SbjId);
        }

        /// <summary>
        /// Get contacts of specified user
        /// </summary>
        /// <param name="client"></param>
        /// <param name="oid">User oid</param>
        /// <returns>ContactInfo array</returns>
        public static async Task<IEnumerable<ContactInfo>> GetPersonContactsAsync(this EsiaClient client, string oid)
        {
            if ( String.IsNullOrEmpty(oid) ) throw new ArgumentNullException("oid");

            string uri = String.Format("{0}{1}/{2}?embed=(elements)", EsiaHelpers.NormalizeUri(client.Options.RestUri, client.Options.PrnsSfx), oid,
                client.Options.CttsSfx);
            var result = new List<ContactInfo>();
            var response = await client.GetAsync(uri);

            if ( response != null )
            {
                IDictionary<string, JToken> contactsDictionary = JObject.Parse(response);

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
            }

            return result;
        }

        /// <summary>
        /// Get addresses of authorized user
        /// </summary>
        /// <param name="client"></param>
        /// <returns>AddrInfo array</returns>
        public static async Task<IEnumerable<AddrInfo>> GetPersonAddrsAsync(this EsiaClient client)
        {
            if ( client.Token == null ) throw new ArgumentNullException("Token");

            return await client.GetPersonAddrsAsync(client.Token.SbjId);
        }

        /// <summary>
        /// Get addresses of specified user
        /// </summary>
        /// <param name="client"></param>
        /// <param name="oid">User oid</param>
        /// <returns>AddrInfo array</returns>
        public static async Task<IEnumerable<AddrInfo>> GetPersonAddrsAsync(this EsiaClient client, string oid)
        {
            if ( String.IsNullOrEmpty(oid) ) throw new ArgumentNullException("oid");

            string uri = String.Format("{0}{1}/{2}?embed=(elements)", EsiaHelpers.NormalizeUri(client.Options.RestUri, client.Options.PrnsSfx), oid,
                client.Options.AddrsSfx);
            var result = new List<AddrInfo>();
            var response = await client.GetAsync(uri);

            if ( response != null )
            {
                IDictionary<string, JToken> addrDictionary = JObject.Parse(response);

                if ( addrDictionary != null && addrDictionary.ContainsKey("elements") )
                {
                    foreach ( var addr in addrDictionary["elements"] )
                    {
                        var addrObject = addr as JObject;

                        if ( addrObject != null ) result.Add(new AddrInfo(addrObject));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Get documents of authorized user
        /// </summary>
        /// <param name="client"></param>
        /// <returns>DocInfo array</returns>
        public static async Task<IEnumerable<DocInfo>> GetPersonDocsAsync(this EsiaClient client)
        {
            if ( client.Token == null ) throw new ArgumentNullException("Token");

            return await client.GetPersonDocsAsync(client.Token.SbjId);
        }

        /// <summary>
        /// Get documents of specified user
        /// </summary>
        /// <param name="client"></param>
        /// <param name="oid">User oid</param>
        /// <returns>DocInfo array</returns>
        public static async Task<IEnumerable<DocInfo>> GetPersonDocsAsync(this EsiaClient client, string oid)
        {
            if ( String.IsNullOrEmpty(oid) ) throw new ArgumentNullException("oid");

            string uri = String.Format("{0}{1}/{2}?embed=(elements)", EsiaHelpers.NormalizeUri(client.Options.RestUri, client.Options.PrnsSfx), oid,
                client.Options.DocsSfx);
            var result = new List<DocInfo>();
            var response = await client.GetAsync(uri);

            if ( response != null )
            {
                IDictionary<string, JToken> docDictionary = JObject.Parse(response);

                if ( docDictionary != null && docDictionary.ContainsKey("elements") )
                {
                    foreach ( var doc in docDictionary["elements"] )
                    {
                        var docObject = doc as JObject;

                        if ( docObject != null ) result.Add(new DocInfo(docObject));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Get children of authorized user
        /// </summary>
        /// <param name="client"></param>
        /// <returns>PersonInfo array</returns>
        public static async Task<IEnumerable<PersonInfo>> GetPersonKidsAsync(this EsiaClient client)
        {
            if ( client.Token == null ) throw new ArgumentNullException("Token");

            return await client.GetPersonKidsAsync(client.Token.SbjId);
        }

        /// <summary>
        /// Get children of specified user
        /// </summary>
        /// <param name="client"></param>
        /// <param name="oid">User oid</param>
        /// <returns>PersonInfo array</returns>
        public static async Task<IEnumerable<PersonInfo>> GetPersonKidsAsync(this EsiaClient client, string oid)
        {
            if ( String.IsNullOrEmpty(oid) ) throw new ArgumentNullException("oid");

            string uri = String.Format("{0}{1}/{2}?embed=(elements)", EsiaHelpers.NormalizeUri(client.Options.RestUri, client.Options.PrnsSfx), oid,
                client.Options.KidsSfx);
            var result = new List<PersonInfo>();
            var response = await client.GetAsync(uri);

            if ( response != null )
            {
                IDictionary<string, JToken> kidsDictionary = JObject.Parse(response);

                if ( kidsDictionary != null && kidsDictionary.ContainsKey("elements") )
                {
                    foreach ( var child in kidsDictionary["elements"] )
                    {
                        var childObject = child as JObject;

                        if ( childObject != null ) result.Add(new PersonInfo(childObject));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Get child documents of authorized user
        /// </summary>
        /// <param name="client"></param>
        /// <returns>DocInfo array</returns>
        public static async Task<IEnumerable<DocInfo>> GetPersonChildDocsAsync(this EsiaClient client, string childOid)
        {
            if ( client.Token == null ) throw new ArgumentNullException("Token");

            return await client.GetPersonChildDocsAsync(client.Token.SbjId, childOid);
        }

        /// <summary>
        /// Get child documents of specified user
        /// </summary>
        /// <param name="client"></param>
        /// <param name="oid">User oid</param>
        /// <returns>DocInfo array</returns>
        public static async Task<IEnumerable<DocInfo>> GetPersonChildDocsAsync(this EsiaClient client, string oid, string childOid)
        {
            if ( String.IsNullOrEmpty(oid) ) throw new ArgumentNullException("oid");

            string uri = String.Format("{0}{1}/{2}/{3}/{4}?embed=(elements)", EsiaHelpers.NormalizeUri(client.Options.RestUri, client.Options.PrnsSfx), oid,
                client.Options.KidsSfx, childOid, client.Options.DocsSfx);
            var result = new List<DocInfo>();
            var response = await client.GetAsync(uri);

            if ( response != null )
            {
                IDictionary<string, JToken> docDictionary = JObject.Parse(response);

                if ( docDictionary != null && docDictionary.ContainsKey("elements") )
                {
                    foreach ( var doc in docDictionary["elements"] )
                    {
                        var docObject = doc as JObject;

                        if ( docObject != null ) result.Add(new DocInfo(docObject));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Get vehicles of authorized user
        /// </summary>
        /// <param name="client"></param>
        /// <returns>VehicleInfo array</returns>
        public static async Task<IEnumerable<VehicleInfo>> GetPersonVehiclesAsync(this EsiaClient client)
        {
            if ( client.Token == null ) throw new ArgumentNullException("Token");

            return await client.GetPersonVehiclesAsync(client.Token.SbjId);
        }

        /// <summary>
        /// Get vehicles of specified user
        /// </summary>
        /// <param name="client"></param>
        /// <param name="oid">User oid</param>
        /// <returns>VehicleInfo array</returns>
        public static async Task<IEnumerable<VehicleInfo>> GetPersonVehiclesAsync(this EsiaClient client, string oid)
        {
            if ( String.IsNullOrEmpty(oid) ) throw new ArgumentNullException("oid");

            string uri = String.Format("{0}{1}/{2}?embed=(elements)", EsiaHelpers.NormalizeUri(client.Options.RestUri, client.Options.PrnsSfx), oid,
                client.Options.VhlsSfx);
            var result = new List<VehicleInfo>();
            var response = await client.GetAsync(uri);

            if ( response != null )
            {
                IDictionary<string, JToken> vehicleDictionary = JObject.Parse(response);

                if ( vehicleDictionary != null && vehicleDictionary.ContainsKey("elements") )
                {
                    foreach ( var vehicle in vehicleDictionary["elements"] )
                    {
                        var vehicleObject = vehicle as JObject;

                        if ( vehicleObject != null ) result.Add(new VehicleInfo(vehicleObject));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Get vehicles of authorized user
        /// </summary>
        /// <param name="client"></param>
        /// <returns>VehicleInfo array</returns>
        public static Task<IEnumerable<PersonRole>> GetPersonRolesAsync(this EsiaClient client)
        {
            if (client.Token == null)
            {
                throw new ArgumentNullException("Token");
            }

            return client.GetPersonRolesAsync(client.Token.SbjId);
        }

        /// <summary>
        /// Get person roles of specified user
        /// </summary>
        /// <param name="client"></param>
        /// <param name="oid">User oid</param>
        /// <returns>VehicleInfo array</returns>
        public static async Task<IEnumerable<PersonRole>> GetPersonRolesAsync(this EsiaClient client, string oid)
        {
            if (string.IsNullOrEmpty(oid))
            {
                throw new ArgumentNullException(nameof(oid));
            }

            var uri = $"{EsiaHelpers.NormalizeUri(client.Options.RestUri, client.Options.PrnsSfx)}{oid}/roles";
            var result = new List<PersonRole>();
            var response = await client.GetAsync(uri);

            if (response == null)
            {
                return result;
            }
            
            IDictionary<string, JToken> vehicleDictionary = JObject.Parse(response);

            if (!vehicleDictionary.ContainsKey("elements"))
            {
                return result;
            }
            
            foreach ( var vehicle in vehicleDictionary["elements"] )
            {
                if (vehicle is JObject personRole)
                {
                    result.Add(new PersonRole(personRole));
                }
            }

            return result;
        }
    }
}