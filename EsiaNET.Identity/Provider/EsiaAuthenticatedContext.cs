// EsiaNET.Identity version 0.1
// 
// Home page: 
// Author: https://github.com/xeltan

using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Provider;

namespace EsiaNET.Identity.Provider
{
    public class EsiaAuthenticatedContext : BaseContext
    {
        public EsiaAuthenticatedContext(IOwinContext context, EsiaToken token, PersonInfo personInfo, IEnumerable<ContactInfo> contactInfo) : base(context)
        {
            if ( token == null ) throw new ArgumentNullException("token");

            Id = token.SbjId;
            Token = token;
            PersonInfo = personInfo;
            PersonContacts = contactInfo;
        }

        public PersonInfo PersonInfo { get; private set; }

        public IEnumerable<ContactInfo> PersonContacts { get; private set; }

        /// <summary>
        /// Gets the ESIA user ID
        /// </summary>
        public string Id { get; private set; }

        public EsiaToken Token { get; }

        /// <summary>
        /// Gets the <see cref="ClaimsIdentity"/> representing the user
        /// </summary>
        public ClaimsIdentity Identity { get; set; }

        /// <summary>
        /// Gets or sets a property bag for common authentication properties
        /// </summary>
        public AuthenticationProperties Properties { get; set; }
    }
}