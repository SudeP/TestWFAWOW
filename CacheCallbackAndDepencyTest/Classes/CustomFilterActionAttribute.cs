using System.Diagnostics;
using System.Reflection;
using System.Web.Mvc;

public class CustomFilterActionAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        //var result = new ContentResult
        //{
        //    Content = "test",
        //    ContentType = "text/html"
        //};
        //filterContext.Result = result;
        Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
        base.OnActionExecuting(filterContext);
    }
    public override void OnActionExecuted(ActionExecutedContext filterContext)
    {
        Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
        base.OnActionExecuted(filterContext);
    }
    public override void OnResultExecuting(ResultExecutingContext filterContext)
    {
        Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
        base.OnResultExecuting(filterContext);
    }
    public override void OnResultExecuted(ResultExecutedContext filterContext)
    {
        Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
        base.OnResultExecuted(filterContext);
    }
}