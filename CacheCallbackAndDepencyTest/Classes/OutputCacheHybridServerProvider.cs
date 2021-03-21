using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Caching;
using System.Web.Caching;

public class OutputCacheHybridServerProvider : OutputCacheProvider
{
    readonly MemoryCache mc = MemoryCache.Default;
    public override object Add(string key, object entry, DateTime utcExpiry)
    {
        Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());

        object @new = null;

        var old = mc.Get(key);
        if (old == null)
        {
            @new = entry;
            mc.Set(key, entry, utcExpiry);
        }

        return @new ?? old;
    }
    public override object Get(string key)
    {
        Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
        var obj = mc.Get(key);
        return obj;
    }
    public override void Remove(string key)
    {
        Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
        mc.Remove(key);
    }
    public override void Set(string key, object entry, DateTime utcExpiry)
    {
        Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());

        mc.Set(key, entry, utcExpiry);
    }
}