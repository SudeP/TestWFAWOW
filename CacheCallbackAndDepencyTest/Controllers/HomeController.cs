using System;
using System.Threading;
using System.Web.Mvc;

namespace CacheCallbackAndDepencyTest.Controllers
{
    public class HomeController : OverrideController
    {
        private Random rand = new Random();
        int strt = 2000;
        int fnsh = 5000;
        public ActionResult TestPage() => View();
        [OutputCache(Duration = 10, VaryByParam = "*")]
        public ActionResult Frame1()
        {
            Thread.Sleep(rand.Next(strt, fnsh));
            return View();
        }
        [OutputCache(Duration = 17, VaryByParam = "*")]
        public ActionResult Frame2()
        {
            Thread.Sleep(rand.Next(strt, fnsh));
            return View();
        }
        [OutputCache(Duration = 100, VaryByParam = "id")]
        public ActionResult Frame3()
        {
            Thread.Sleep(rand.Next(strt, fnsh));
            return View();
        }
        [OutputCache(Duration = 15, VaryByParam = "*")]
        public ActionResult Frame4()
        {
            Thread.Sleep(rand.Next(strt, fnsh));
            return View();
        }





        private ActionResult test()
        {
            Thread.Sleep(rand.Next(strt, fnsh));
            return View("test");
        }
        [OutputCache(Duration = 10, VaryByParam = "*")]
        public ActionResult test1() => test();
        [OutputCache(Duration = 10, VaryByParam = "*")]
        public ActionResult test2() => test();
        [OutputCache(Duration = 10, VaryByParam = "*")]
        public ActionResult test3() => test();
        [OutputCache(Duration = 10, VaryByParam = "*")]
        public ActionResult test4() => test();
        [OutputCache(Duration = 10, VaryByParam = "*")]
        public ActionResult test5() => test();
        [OutputCache(Duration = 10, VaryByParam = "*")]
        public ActionResult test6() => test();
        [OutputCache(Duration = 10, VaryByParam = "*")]
        public ActionResult test7() => test();
        [OutputCache(Duration = 10, VaryByParam = "*")]
        public ActionResult test8() => test();
        [OutputCache(Duration = 10, VaryByParam = "*")]
        public ActionResult test9() => test();
        [OutputCache(Duration = 10, VaryByParam = "*")]
        public ActionResult test10() => test();
        [OutputCache(Duration = 10, VaryByParam = "*")]
        public ActionResult test11() => test();
        [OutputCache(Duration = 10, VaryByParam = "*")]
        public ActionResult test12() => test();
        [OutputCache(Duration = 10, VaryByParam = "*")]
        public ActionResult test13() => test();
        [OutputCache(Duration = 10, VaryByParam = "*")]
        public ActionResult test14() => test();
        [OutputCache(Duration = 10, VaryByParam = "*")]
        public ActionResult test15() => test();
        [OutputCache(Duration = 10, VaryByParam = "*")]
        public ActionResult test16() => test();
        [OutputCache(Duration = 10, VaryByParam = "*")]
        public ActionResult test17() => test();
        [OutputCache(Duration = 10, VaryByParam = "*")]
        public ActionResult test18() => test();
        [OutputCache(Duration = 10, VaryByParam = "*")]
        public ActionResult test19() => test();
        [OutputCache(Duration = 10, VaryByParam = "*")]
        public ActionResult test20() => test();
        [OutputCache(Duration = 10, VaryByParam = "*")]
        public ActionResult test21() => test();
    }
}