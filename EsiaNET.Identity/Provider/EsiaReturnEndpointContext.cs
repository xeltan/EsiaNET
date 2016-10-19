// EsiaNET.Identity version 0.1
// 
// Home page: 
// Author: https://github.com/xeltan

using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Provider;

namespace EsiaNET.Identity.Provider
{
    public class EsiaReturnEndpointContext : ReturnEndpointContext
    {
        public EsiaReturnEndpointContext(IOwinContext context, AuthenticationTicket ticket) : base(context, ticket)
        {
        }
    }
}