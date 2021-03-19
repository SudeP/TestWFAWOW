using System;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace CacheCallbackAndDepencyTest.Controllers
{
    public class HomeController : Controller
    {
        [HttpPost]
        public JsonResult Control()
        {
            return Json(MC._1second.Updatable(
                "name",
                DateTime.Now.AddSeconds(5),
                false,
                () => $"{DateTime.Now:HH:mm:ss}"
            ));
        }
        public ActionResult Index() => View();
        public ActionResult Action() => View();
        public ActionResult FrameMain() => View();
        public ActionResult Frame1() => View();
        public ActionResult Frame2() => View();
        public ActionResult Frame3() => View();
        public ActionResult Frame4() => View();
        protected new ViewResult View(bool usLayout = false, [CallerMemberName] string name = null)
        {
            var vr = View(name);
            vr.MasterName = "~/Views/Layout.cshtml";
            return vr;
        }
    }
}