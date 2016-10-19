using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ESIA.AspNetIdentityExample.Startup))]
namespace ESIA.AspNetIdentityExample
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
