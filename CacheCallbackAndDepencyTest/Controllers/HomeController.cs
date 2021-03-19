using System.Web.Mvc;

namespace CacheCallbackAndDepencyTest.Controllers
{
    [System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class OutputCacheOverrideAttribute : OutputCacheAttribute
    {
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
    }
    public class HomeController : OverrideController
    {
        public ActionResult TestPage() => View();
        [OutputCacheOverride(Duration = 600, VaryByParam = "*")]
        public ActionResult Frame1() => View();
        [OutputCacheOverride(Duration = 1800, VaryByParam = "*")]
        public ActionResult Frame2() => View();
        [OutputCacheOverride(Duration = 120)]
        public ActionResult Frame3() => View();
        [OutputCacheOverride(Duration = 9000, VaryByParam = "*")]
        public ActionResult Frame4() => View();
    }
}