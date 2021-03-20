using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Routing;

namespace CacheCallbackAndDepencyTest.Controllers
{
    public class OverrideController : Controller
    {
        protected new ViewResult View([CallerMemberName] string name = null)
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
            return View(name, "~/Views/Layout.cshtml");
        }

        protected override IAsyncResult BeginExecute(RequestContext requestContext, AsyncCallback callback, object state)
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
            return base.BeginExecute(requestContext, callback, state);
        }
        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
            return base.BeginExecuteCore(callback, state);
        }
        protected override IActionInvoker CreateActionInvoker()
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
            return base.CreateActionInvoker();
        }
        protected override ITempDataProvider CreateTempDataProvider()
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
            return base.CreateTempDataProvider();
        }
        protected override void EndExecute(IAsyncResult asyncResult)
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
            base.EndExecute(asyncResult);
        }
        protected override void EndExecuteCore(IAsyncResult asyncResult)
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
            base.EndExecuteCore(asyncResult);
        }
        protected override void Execute(RequestContext requestContext)
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
            base.Execute(requestContext);
        }
        protected override void ExecuteCore()
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
            base.ExecuteCore();
        }
        protected override void HandleUnknownAction(string actionName)
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
            base.HandleUnknownAction(actionName);
        }
        protected override HttpNotFoundResult HttpNotFound(string statusDescription)
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
            return base.HttpNotFound(statusDescription);
        }
        protected override void Initialize(RequestContext requestContext)
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
            base.Initialize(requestContext);
        }
        protected override ContentResult Content(string content, string contentType, Encoding contentEncoding)
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
            return base.Content(content, contentType, contentEncoding);
        }
        protected override bool DisableAsyncSupport
        {
            get
            {
                Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
                return base.DisableAsyncSupport;
            }
        }

        protected override void Dispose(bool disposing)
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
            base.Dispose(disposing);
        }
        public override bool Equals(object obj)
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
            return base.Equals(obj);
        }
        protected override FileContentResult File(byte[] fileContents, string contentType, string fileDownloadName)
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
            return base.File(fileContents, contentType, fileDownloadName);
        }
        protected override FilePathResult File(string fileName, string contentType, string fileDownloadName)
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
            return base.File(fileName, contentType, fileDownloadName);
        }
        protected override FileStreamResult File(Stream fileStream, string contentType, string fileDownloadName)
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
            return base.File(fileStream, contentType, fileDownloadName);
        }
        public override int GetHashCode()
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
            return base.GetHashCode();
        }
        protected override JavaScriptResult JavaScript(string script)
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
            return base.JavaScript(script);
        }
        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding)
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
            return base.Json(data, contentType, contentEncoding);
        }
        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
            return base.Json(data, contentType, contentEncoding, behavior);
        }
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
            base.OnActionExecuted(filterContext);
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
            base.OnActionExecuting(filterContext);
        }
        protected override void OnAuthentication(AuthenticationContext filterContext)
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
            base.OnAuthentication(filterContext);
        }
        protected override void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
            base.OnAuthenticationChallenge(filterContext);
        }
        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
            base.OnAuthorization(filterContext);
        }
        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
        }
        protected override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
            base.OnResultExecuted(filterContext);
        }
        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
            base.OnResultExecuting(filterContext);
        }
        protected override PartialViewResult PartialView(string viewName, object model)
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
            return base.PartialView(viewName, model);
        }
        protected override RedirectResult Redirect(string url)
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
            return base.Redirect(url);
        }
        protected override RedirectResult RedirectPermanent(string url)
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
            return base.RedirectPermanent(url);
        }
        protected override RedirectToRouteResult RedirectToAction(string actionName, string controllerName, RouteValueDictionary routeValues)
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
            return base.RedirectToAction(actionName, controllerName, routeValues);
        }
        protected override RedirectToRouteResult RedirectToActionPermanent(string actionName, string controllerName, RouteValueDictionary routeValues)
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
            return base.RedirectToActionPermanent(actionName, controllerName, routeValues);
        }
        protected override RedirectToRouteResult RedirectToRoute(string routeName, RouteValueDictionary routeValues)
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
            return base.RedirectToRoute(routeName, routeValues);
        }
        protected override RedirectToRouteResult RedirectToRoutePermanent(string routeName, RouteValueDictionary routeValues)
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
            return base.RedirectToRoutePermanent(routeName, routeValues);
        }
        public override string ToString()
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
            return base.ToString();
        }
        protected override ViewResult View(IView view, object model)
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
            return base.View(view, model);
        }
        protected override ViewResult View(string viewName, string masterName, object model)
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
            return base.View(viewName, masterName, model);
        }
    }
}