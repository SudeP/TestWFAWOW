﻿using System.Web.Mvc;

namespace CacheCallbackAndDepencyTest
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new CustomFilterActionAttribute());
        }
    }
}