using System.Threading;
using System.Web.Mvc;

namespace CacheCallbackAndDepencyTest.Controllers
{
    public class HomeController : OverrideController
    {
        public ActionResult TestPage() => View();
        [OutputCache(Duration = 600, VaryByParam = "*")]
        public ActionResult Frame1() => View();
















        [OutputCache(Duration = 10, VaryByParam = "*")]
        public ActionResult Frame2()
        {
            Thread.Sleep(5000);
            return View();
        }

        [OutputCache(Duration = 60000, VaryByParam = "*")]
        public ActionResult Frame3() => View();









        [OutputCache(Duration = 10, VaryByParam = "*")]
        public ActionResult Frame4() => View();
    }
}