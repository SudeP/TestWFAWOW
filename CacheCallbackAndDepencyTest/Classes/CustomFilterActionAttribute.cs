using System;
using System.Diagnostics;
using System.Reflection;
using System.Web.Mvc;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class CustomFilterActionAttribute : ActionFilterAttribute, IActionFilter
{
    public override void OnActionExecuted(ActionExecutedContext filterContext)
    {
        Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
        base.OnActionExecuted(filterContext);
    }
    public override void OnResultExecuted(ResultExecutedContext filterContext)
    {
        Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
        base.OnResultExecuted(filterContext);
    }
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
        base.OnActionExecuting(filterContext);
    }
    public override void OnResultExecuting(ResultExecutingContext filterContext)
    {
        Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
        base.OnResultExecuting(filterContext);
    }
    void IActionFilter.OnActionExecuted(ActionExecutedContext filterContext)
    {
        Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
    }
    void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
    {
        Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
    }
}