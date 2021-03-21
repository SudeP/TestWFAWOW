using System.Web.Mvc;

namespace CacheCallbackAndDepencyTest.Controllers
{
    public class HomeController : OverrideController
    {
        public ActionResult TestPage() => View();
        [OutputCacheSpec(Duration = 600, VaryByParam = "*")]
        public ActionResult Frame1() => View();
        [OutputCacheSpec(Duration = 1800, VaryByParam = "*")]
        public ActionResult Frame2() => View();
        [OutputCacheSpec(Duration = 120, VaryByParam = "*")]
        public ActionResult Frame3() => View();
        [OutputCacheSpec(Duration = 10, VaryByParam = "*")]
        public ActionResult Frame4() => View();
    }
}