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
            //PreSendRequestContent += MvcApplication_PreSendRequestContent;
            //EndRequest += MvcApplication_EndRequest;
            //Error += MvcApplication_Error;
            //RequestCompleted += MvcApplication_RequestCompleted;
            //PostLogRequest += MvcApplication_PostLogRequest;
            //PreSendRequestHeaders += MvcApplication_PreSendRequestHeaders;
            //Disposed += MvcApplication_Disposed;
            //AuthenticateRequest += MvcApplication_AuthenticateRequest;
            //UpdateRequestCache += MvcApplication_UpdateRequestCache;
            //PostReleaseRequestState += MvcApplication_PostReleaseRequestState;
            //ReleaseRequestState += MvcApplication_ReleaseRequestState;
            //PostRequestHandlerExecute += MvcApplication_PostRequestHandlerExecute;
            //PreRequestHandlerExecute += MvcApplication_PreRequestHandlerExecute;
            //PostAcquireRequestState += MvcApplication_PostAcquireRequestState;
            //BeginRequest += MvcApplication_BeginRequest;
            //AcquireRequestState += MvcApplication_AcquireRequestState;
            //MapRequestHandler += MvcApplication_MapRequestHandler;
            //PostResolveRequestCache += MvcApplication_PostResolveRequestCache;
            //ResolveRequestCache += MvcApplication_ResolveRequestCache;
            //PostAuthorizeRequest += MvcApplication_PostAuthorizeRequest;
            //AuthorizeRequest += MvcApplication_AuthorizeRequest;
            //PostAuthenticateRequest += MvcApplication_PostAuthenticateRequest;
            //PostMapRequestHandler += MvcApplication_PostMapRequestHandler;
            //PostUpdateRequestCache += MvcApplication_PostUpdateRequestCache;
            //LogRequest += MvcApplication_LogRequest;
        }
        private void MvcApplication_LogRequest(object sender, System.EventArgs e)
        {

        }
        private void MvcApplication_PostUpdateRequestCache(object sender, System.EventArgs e)
        {

        }
        private void MvcApplication_PostMapRequestHandler(object sender, System.EventArgs e)
        {

        }
        private void MvcApplication_PostAuthenticateRequest(object sender, System.EventArgs e)
        {

        }
        private void MvcApplication_PostAuthorizeRequest(object sender, System.EventArgs e)
        {

        }
        private void MvcApplication_ResolveRequestCache(object sender, System.EventArgs e)
        {

        }
        private void MvcApplication_PostResolveRequestCache(object sender, System.EventArgs e)
        {

        }
        private void MvcApplication_MapRequestHandler(object sender, System.EventArgs e)
        {

        }
        private void MvcApplication_BeginRequest(object sender, System.EventArgs e)
        {

        }
        private void MvcApplication_PostAcquireRequestState(object sender, System.EventArgs e)
        {

        }
        private void MvcApplication_PreRequestHandlerExecute(object sender, System.EventArgs e)
        {

        }
        private void MvcApplication_PostRequestHandlerExecute(object sender, System.EventArgs e)
        {

        }
        private void MvcApplication_ReleaseRequestState(object sender, System.EventArgs e)
        {

        }
        private void MvcApplication_PostReleaseRequestState(object sender, System.EventArgs e)
        {

        }
        private void MvcApplication_UpdateRequestCache(object sender, System.EventArgs e)
        {

        }
        private void MvcApplication_Disposed(object sender, System.EventArgs e)
        {

        }
        private void MvcApplication_PreSendRequestHeaders(object sender, System.EventArgs e)
        {

        }
        private void MvcApplication_PostLogRequest(object sender, System.EventArgs e)
        {

        }
        private void MvcApplication_RequestCompleted(object sender, System.EventArgs e)
        {

        }
        private void MvcApplication_Error(object sender, System.EventArgs e)
        {

        }
        private void MvcApplication_EndRequest(object sender, System.EventArgs e)
        {

        }
        private void MvcApplication_PreSendRequestContent(object sender, System.EventArgs e)
        {

        }
        private void MvcApplication_AuthorizeRequest(object sender, System.EventArgs e)
        {

        }
        private void MvcApplication_AuthenticateRequest(object sender, System.EventArgs e)
        {

        }
        private void MvcApplication_AcquireRequestState(object sender, System.EventArgs e)
        {

        }
    }
}
