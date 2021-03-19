﻿using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Routing;

namespace CacheCallbackAndDepencyTest.Controllers
{
    [System.AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class OutputCacheOverrideAttribute : OutputCacheAttribute
    {
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override string ToString()
        {
            return base.ToString();
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool IsDefaultAttribute()
        {
            return base.IsDefaultAttribute();
        }
        public override bool Match(object obj)
        {
            return base.Match(obj);
        }
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
        }
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
        }
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);
        }
        public override object TypeId => base.TypeId;
    }
    public class OverrideController : Controller
    {
        protected new ViewResult View([CallerMemberName] string name = null) => View(name, "~/Views/Layout.cshtml");
        protected override IAsyncResult BeginExecute(RequestContext requestContext, AsyncCallback callback, object state)
        {
            return base.BeginExecute(requestContext, callback, state);
        }
        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            return base.BeginExecuteCore(callback, state);
        }
        protected override IActionInvoker CreateActionInvoker()
        {
            return base.CreateActionInvoker();
        }
        protected override ITempDataProvider CreateTempDataProvider()
        {
            return base.CreateTempDataProvider();
        }
        protected override void EndExecute(IAsyncResult asyncResult)
        {
            base.EndExecute(asyncResult);
        }
        protected override void EndExecuteCore(IAsyncResult asyncResult)
        {
            base.EndExecuteCore(asyncResult);
        }
        protected override void Execute(RequestContext requestContext)
        {
            base.Execute(requestContext);
        }
        protected override void ExecuteCore()
        {
            base.ExecuteCore();
        }
        protected override void HandleUnknownAction(string actionName)
        {
            base.HandleUnknownAction(actionName);
        }
        protected override HttpNotFoundResult HttpNotFound(string statusDescription)
        {
            return base.HttpNotFound(statusDescription);
        }
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
        }
        protected override ContentResult Content(string content, string contentType, Encoding contentEncoding)
        {
            return base.Content(content, contentType, contentEncoding);
        }
        protected override bool DisableAsyncSupport => base.DisableAsyncSupport;
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        protected override FileContentResult File(byte[] fileContents, string contentType, string fileDownloadName)
        {
            return base.File(fileContents, contentType, fileDownloadName);
        }
        protected override FilePathResult File(string fileName, string contentType, string fileDownloadName)
        {
            return base.File(fileName, contentType, fileDownloadName);
        }
        protected override FileStreamResult File(Stream fileStream, string contentType, string fileDownloadName)
        {
            return base.File(fileStream, contentType, fileDownloadName);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        protected override JavaScriptResult JavaScript(string script)
        {
            return base.JavaScript(script);
        }
        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding)
        {
            return base.Json(data, contentType, contentEncoding);
        }
        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return base.Json(data, contentType, contentEncoding, behavior);
        }
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
        }
        protected override void OnAuthentication(AuthenticationContext filterContext)
        {
            base.OnAuthentication(filterContext);
        }
        protected override void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            base.OnAuthenticationChallenge(filterContext);
        }
        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
        }
        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
        }
        protected override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
        }
        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);
        }
        protected override PartialViewResult PartialView(string viewName, object model)
        {
            return base.PartialView(viewName, model);
        }
        protected override RedirectResult Redirect(string url)
        {
            return base.Redirect(url);
        }
        protected override RedirectResult RedirectPermanent(string url)
        {
            return base.RedirectPermanent(url);
        }
        protected override RedirectToRouteResult RedirectToAction(string actionName, string controllerName, RouteValueDictionary routeValues)
        {
            return base.RedirectToAction(actionName, controllerName, routeValues);
        }
        protected override RedirectToRouteResult RedirectToActionPermanent(string actionName, string controllerName, RouteValueDictionary routeValues)
        {
            return base.RedirectToActionPermanent(actionName, controllerName, routeValues);
        }
        protected override RedirectToRouteResult RedirectToRoute(string routeName, RouteValueDictionary routeValues)
        {
            return base.RedirectToRoute(routeName, routeValues);
        }
        protected override RedirectToRouteResult RedirectToRoutePermanent(string routeName, RouteValueDictionary routeValues)
        {
            return base.RedirectToRoutePermanent(routeName, routeValues);
        }
        public override string ToString()
        {
            return base.ToString();
        }
        protected override ViewResult View(IView view, object model)
        {
            return base.View(view, model);
        }
        protected override ViewResult View(string viewName, string masterName, object model)
        {
            return base.View(viewName, masterName, model);
        }
    }
}