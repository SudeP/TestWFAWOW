using System.Diagnostics;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;

namespace CacheCallbackAndDepencyTest
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{action}",
                defaults: new { controller = "Home", action = "TestPage" }
            );
        }
    }
}
