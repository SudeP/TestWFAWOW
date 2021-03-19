using System;
using System.Runtime.Caching;
using System.Web.Caching;

public class OutputCacheHybridServerProvider : OutputCacheProvider
{
    readonly MemoryCache mc = MemoryCache.Default;
    public override object Add(string key, object entry, DateTime utcExpiry)
    {
        mc.Add(key, entry, utcExpiry.ToLocalTime());
        return entry;
    }
    public override object Get(string key)
    {
        return mc.Get(key);
    }
    public override void Remove(string key)
    {
        mc.Remove(key);
    }
    public override void Set(string key, object entry, DateTime utcExpiry)
    {
        mc.Set(key, entry, utcExpiry.ToLocalTime());
    }
}