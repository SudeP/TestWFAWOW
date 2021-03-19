using System;
using System.Web.Mvc;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class CustomFilterActionAttribute : ActionFilterAttribute, IActionFilter
{
    public override void OnActionExecuted(ActionExecutedContext filterContext)
    {
        base.OnActionExecuted(filterContext);
    }
    public override void OnResultExecuted(ResultExecutedContext filterContext)
    {
        base.OnResultExecuted(filterContext);
    }
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        base.OnActionExecuting(filterContext);
    }
    public override void OnResultExecuting(ResultExecutingContext filterContext)
    {
        base.OnResultExecuting(filterContext);
    }
    void IActionFilter.OnActionExecuted(ActionExecutedContext filterContext)
    {

    }
    void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
    {

    }
}