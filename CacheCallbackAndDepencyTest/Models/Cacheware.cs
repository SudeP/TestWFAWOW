using System;
using System.Reflection;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Web.Caching;

public class CustomOutputCacheProvider : OutputCacheProviderAsync
{
    private readonly static MemoryCache _cache = MemoryCache.Default;

    /// <summary>
    /// Asynchronously inserts the specified entry into the output cache.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="entry"></param>
    /// <param name="utcExpiry"></param>
    /// <returns></returns>
    public override Task<object> AddAsync(string key, object entry, DateTime utcExpiry)
    {
        //TODO:
        //Replace with your own async data insertion mechanism.
        DateTimeOffset expiration = (utcExpiry == Cache.NoAbsoluteExpiration) ? ObjectCache.InfiniteAbsoluteExpiration : utcExpiry;
        return Task.FromResult(_cache.AddOrGetExisting(key, entry, expiration));
    }

    /// <summary>
    /// Asynchronously returns a reference to the specified entry in the output cache.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public override Task<object> GetAsync(string key)
    {
        //TODO:
        //Replace with your own aysnc data retrieve mechanism.
        return Task.FromResult(_cache.Get(key));
    }

    /// <summary>
    /// Asynchronously Inserts the specified entry into the output cache, overwriting the entry if it is already cached.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="entry"></param>
    /// <param name="utcExpiry"></param>
    /// <returns></returns>
    public override Task SetAsync(string key, object entry, DateTime utcExpiry)
    {
        //TODO:
        //Replace with your own async insertion/overwriting mechanism.
        DateTimeOffset expiration = (utcExpiry == Cache.NoAbsoluteExpiration) ? ObjectCache.InfiniteAbsoluteExpiration : utcExpiry;
        _cache.Set(key, entry, expiration);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Asynchronously removes the specified entry from the output cache.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public override Task RemoveAsync(string key)
    {
        //TODO:
        //Replace with your own async data removal mechanism.
        _cache.Remove(key);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Returns a reference to the specified entry in the output cache.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public override object Get(string key)
    {
        //TODO:
        //Replace with your own data retrieve mechanism.
        return _cache.Get(key);
    }

    /// <summary>
    /// Inserts the specified entry into the output cache.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="entry"></param>
    /// <param name="utcExpiry"></param>
    /// <returns></returns>
    public override object Add(string key, object entry, DateTime utcExpiry)
    {
        //TODO:
        //Replace with your own data insertion mechanism.
        DateTimeOffset expiration = (utcExpiry == Cache.NoAbsoluteExpiration) ? ObjectCache.InfiniteAbsoluteExpiration : utcExpiry;
        return _cache.AddOrGetExisting(key, entry, expiration);
    }

    /// <summary>
    /// Inserts the specified entry into the output cache, overwriting the entry if it is already cached
    /// </summary>
    /// <param name="key"></param>
    /// <param name="entry"></param>
    /// <param name="utcExpiry"></param>
    public override void Set(string key, object entry, DateTime utcExpiry)
    {
        //TODO:
        //Replace with your own insertion/overwriting mechanism.
        DateTimeOffset expiration = (utcExpiry == Cache.NoAbsoluteExpiration) ? ObjectCache.InfiniteAbsoluteExpiration : utcExpiry;
        _cache.Set(key, entry, expiration);
    }

    /// <summary>
    /// Removes the specified entry from the output cache.
    /// </summary>
    /// <param name="key"></param>
    public override void Remove(string key)
    {
        //TODO:
        //Replace with your own data removal mechanism.
        _cache.Remove(key);
    }
}
public class TestAsyncProvider : OutputCacheProviderAsync
{
    public override object Add(string key, object entry, DateTime utcExpiry)
    {
        return null;
    }
    public override Task<object> AddAsync(string key, object entry, DateTime utcExpiry)
    {
        return null;
    }
    public override object Get(string key)
    {
        return null;
    }
    public override Task<object> GetAsync(string key)
    {
        return null;
    }
    public override void Remove(string key)
    {
    }
    public override Task RemoveAsync(string key)
    {
        return null;
    }
    public override void Set(string key, object entry, DateTime utcExpiry)
    {
    }
    public override Task SetAsync(string key, object entry, DateTime utcExpiry)
    {
        return null;
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