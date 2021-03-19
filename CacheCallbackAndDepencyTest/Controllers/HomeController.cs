using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace CacheCallbackAndDepencyTest.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult TestPage() => View();
        protected new ViewResult View([CallerMemberName] string name = null) => View(name, "~/Views/Layout.cshtml");
        #region FRAMES
        [OutputCache(Duration = 600, VaryByParam = "*")]
        public ActionResult Frame1() => View();
        [OutputCache(Duration = 1800, VaryByParam = "*")]
        public ActionResult Frame2() => View();
        [OutputCache(Duration = 3000, VaryByParam = "*")]
        public ActionResult Frame3() => View();
        [OutputCache(Duration = 9000, VaryByParam = "*")]
        public ActionResult Frame4() => View();
        #endregion
    }
}