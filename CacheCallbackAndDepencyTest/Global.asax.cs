using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CacheCallbackAndDepencyTest
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        }
        protected void Application_ResolveRequestCache(object sender, EventArgs e)
        {
        }
        protected void Application_UpdateRequestCache(object sender, EventArgs e)
        {
        }
        protected void Application_PostResolveRequestCache(object sender, EventArgs e)
        {
        }
        protected void Application_PostUpdateRequestCache(object sender, EventArgs e)
        {
        }
    }
}
