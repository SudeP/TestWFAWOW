using System;
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
        public ActionResult Index()
        {
            return View();
        }
        [OutputCacheMC(Duration = 5)]
        public ActionResult Action()
        {
            return View();
        }
    }
}