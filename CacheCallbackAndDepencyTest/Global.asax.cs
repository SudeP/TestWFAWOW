using System;
using System.Diagnostics;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CacheCallbackAndDepencyTest
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
