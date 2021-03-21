using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace CacheCallbackAndDepencyTest.Controllers
{
    public class OverrideController : Controller
    {
        protected new ViewResult View([CallerMemberName] string name = null)
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
            return View(name, "~/Views/Layout.cshtml");
        }
    }
}