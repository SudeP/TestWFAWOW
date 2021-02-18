using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
public sealed class OutputCacheMC : OutputCacheAttribute
{
    public override void OnResultExecuting(ResultExecutingContext filterContext)
    {
        //ResultExecutingContext fc = filterContext;

        //bool skip = VaryByParam == "*" || string.IsNullOrEmpty(VaryByParam);

        //string[] filterParams = VaryByParam.Split(';');

        //string cacheKey = $"?cache=special";

        //foreach (string queryKey in fc.HttpContext.Request.QueryString)
        //{
        //    if (skip || filterParams.Contains(queryKey))
        //    {
        //        string queryValue = fc.HttpContext.Request.QueryString[queryKey];
        //        cacheKey += $"&{queryKey}={queryValue}";
        //    }
        //}
        //var vr = fc.Result as ViewResult;

        //fc.Result.ExecuteResult(fc.Controller.ControllerContext);

        base.OnResultExecuting(filterContext);
    }
}
public class FileCacheProvider : OutputCacheProvider
{
    public override object Add(string key, object entry, DateTime utcExpiry)
    {
        object obj = MC.Default.Get(key);
        if (obj != null)
        {
            return obj;
        }
        else
        {
            MC.Default.Set(key, entry, utcExpiry);
            return entry;
        }
    }
    public override object Get(string key)
    {
        var obj = MC.Default.Get(key);
        return obj;
    }
    public override void Remove(string key)
    {
        MC.Default.Remove(key);
    }
    public override void Set(string key, object entry, DateTime utcExpiry)
    {
        var e = entry as IOutputCacheEntry;
        MC.Default.Updatable(key, utcExpiry, true,
            () =>
            {
                e.ResponseElements.Clear();

                e.ResponseElements.Add(null);
                return "";
            }
        );
    }
}






















public static class MC
{
#pragma warning disable IDE1006
    private static MemoryCache __1second = null;
    public static MemoryCache Default => MemoryCache.Default;
    public static MemoryCache _1second
    {
        get
        {
            if (__1second is null)
                __1second = CacheExtended.CreateSpecial("1MemoryCache", 1);

            return __1second;
        }
    }
#pragma warning restore IDE1006
}
public static class CacheExtended
{
    public static MemoryCache CreateSpecial(string name, double periodSecond)
    {
        MemoryCache memoryCache = null;

        Assembly assembly = typeof(CacheItemPolicy).Assembly;

        Type type = assembly.GetType("System.Runtime.Caching.CacheExpires");

        if (type != null)
        {
            FieldInfo field = type.GetField("_tsPerBucket", BindingFlags.Static | BindingFlags.NonPublic);

            if (field != null && field.FieldType == typeof(TimeSpan))
            {
                TimeSpan originalValue = (TimeSpan)field.GetValue(null);

                field.SetValue(null, TimeSpan.FromSeconds(periodSecond));

                memoryCache = new MemoryCache(name);

                field.SetValue(null, originalValue);
            }
        }

        return memoryCache;
    }
    public static T GetOrSet<T>(
        this MemoryCache memoryCache,
        string key,
        DateTime absoluteExpiration,
        Func<T> setFunc,
        bool allowNullValue) where T : class
    {
        if (memoryCache is null)
            return default;

        T t = default;

        if (memoryCache.Contains(key))
            t = (T)memoryCache.Get(key);
        else if (AllowNullValueControl(allowNullValue, setFunc, out t))
            memoryCache.Set(new CacheItem(key, t), new CacheItemPolicy
            {
                AbsoluteExpiration = absoluteExpiration,
                SlidingExpiration = new TimeSpan()
            });
        return t;
    }
    public static T Updatable<T>(
        this MemoryCache memoryCache,
        string key,
        DateTime absoluteExpiration,
        bool allowNullValue,
        Func<T> setFunc)
    {
        if (memoryCache is null)
            return default;

        T t = default;

        if (memoryCache.Contains(key))
            t = (T)memoryCache.Get(key);
        else if (AllowNullValueControl(allowNullValue, setFunc, out t))
            memoryCache.Set(new CacheItem(key, t), new CacheItemPolicy
            {
                AbsoluteExpiration = absoluteExpiration,
                SlidingExpiration = new TimeSpan(),
                UpdateCallback = args =>
                {
                    if (args.RemovedReason == CacheEntryRemovedReason.Expired && AllowNullValueControl(allowNullValue, setFunc, out T newValue))
                        args.UpdatedCacheItem = new CacheItem(key, newValue);
                }
            });

        return t;
    }
    private static bool AllowNullValueControl<T>(bool allowNullValue, Func<T> setFunc, out T t)
    {
        t = setFunc.Invoke();
        return allowNullValue ? allowNullValue : t != null;
    }
}