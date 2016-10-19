using System.Web;
using System.Web.Mvc;

namespace ESIA.AspNetIdentityExample
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
