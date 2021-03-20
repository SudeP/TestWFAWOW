using System.Diagnostics;
using System.Reflection;
using System.Web.Mvc;

namespace CacheCallbackAndDepencyTest
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
            filters.Add(new CustomFilterActionAttribute());
        }
    }
}
