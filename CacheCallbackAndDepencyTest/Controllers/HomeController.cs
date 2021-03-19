using System.Web.Mvc;

namespace CacheCallbackAndDepencyTest.Controllers
{
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