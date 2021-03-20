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
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        }
        protected void Session_Start(object sender, EventArgs e) => Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
        protected void Session_End(object sender, EventArgs e) => Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
        protected void Application_End(object sender, EventArgs e) => Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
        protected void Application_PreSendRequestContent(object sender, EventArgs e) => Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
        protected void Application_EndRequest(object sender, EventArgs e) => Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
        protected void Application_Error(object sender, EventArgs e) => Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
        protected void Application_RequestCompleted(object sender, EventArgs e) => Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
        protected void Application_PostLogRequest(object sender, EventArgs e) => Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
        protected void Application_PreSendRequestHeaders(object sender, EventArgs e) => Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
        protected void Application_Disposed(object sender, EventArgs e) => Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
        protected void Application_AuthenticateRequest(object sender, EventArgs e) => Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
        protected void Application_UpdateRequestCache(object sender, EventArgs e) => Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
        protected void Application_PostReleaseRequestState(object sender, EventArgs e) => Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
        protected void Application_ReleaseRequestState(object sender, EventArgs e) => Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
        protected void Application_PostRequestHandlerExecute(object sender, EventArgs e) => Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
        protected void Application_PreRequestHandlerExecute(object sender, EventArgs e) => Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
        protected void Application_PostAcquireRequestState(object sender, EventArgs e) => Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
        protected void Application_BeginRequest(object sender, EventArgs e) => Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
        protected void Application_AcquireRequestState(object sender, EventArgs e) => Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
        protected void Application_MapRequestHandler(object sender, EventArgs e) => Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
        protected void Application_PostResolveRequestCache(object sender, EventArgs e) => Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
        protected void Application_ResolveRequestCache(object sender, EventArgs e) => Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
        protected void Application_PostAuthorizeRequest(object sender, EventArgs e) => Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
        protected void Application_AuthorizeRequest(object sender, EventArgs e) => Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
        protected void Application_PostAuthenticateRequest(object sender, EventArgs e) => Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
        protected void Application_PostMapRequestHandler(object sender, EventArgs e) => Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
        protected void Application_PostUpdateRequestCache(object sender, EventArgs e) => Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
        protected void Application_LogRequest(object sender, EventArgs e) => Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
    }
}
