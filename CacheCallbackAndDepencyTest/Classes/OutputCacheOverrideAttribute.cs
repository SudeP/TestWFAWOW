using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Caching;
using System.Web.Mvc;
using System.Web.UI;

[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
public sealed class OutputCacheOverrideAttribute1 : ActionFilterAttribute, IExceptionFilter
{
    public static ObjectCache ChildActionCache { get; set; }
    public string CacheProfile { get; set; }
    public string VaryByHeader { get; set; }
    public int Duration { get; set; }
    public OutputCacheLocation Location { get; set; }
    public bool NoStore { get; set; }
    public string SqlDependency { get; set; }
    public string VaryByContentEncoding { get; set; }
    public string VaryByCustom { get; set; }
    public string VaryByParam { get; set; }
    public static bool IsChildActionCacheActive(ControllerContext controllerContext)
    {
        return false;
    }
    public override void OnActionExecuted(ActionExecutedContext filterContext)
    {

    }
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {

    }
    public override void OnResultExecuted(ResultExecutedContext filterContext)
    {

    }
    public override void OnResultExecuting(ResultExecutingContext filterContext)
    {

    }
    public void OnException(ExceptionContext filterContext)
    {

    }
}

[System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = true)]
public sealed class OutputCacheOverrideAttribute : OutputCacheAttribute
{
    public override void OnActionExecuted(ActionExecutedContext filterContext)
    {
        Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
        base.OnActionExecuted(filterContext);
    }
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
        base.OnActionExecuting(filterContext);
    }
    public override void OnResultExecuted(ResultExecutedContext filterContext)
    {
        Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
        base.OnResultExecuted(filterContext);
    }
    public override void OnResultExecuting(ResultExecutingContext filterContext)
    {
        Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
        base.OnResultExecuting(filterContext);
    }
}