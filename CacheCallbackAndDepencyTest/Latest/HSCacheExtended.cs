using System;
using System.Reflection;
using System.Runtime.Caching;

namespace HybridServer
{
    public static class HSCacheExtended
    {
        public static MemoryCache CreateSpecial(string name, byte periodSecond)
        {
            MemoryCache memoryCache = null;

            if (periodSecond < 1)
                memoryCache = new MemoryCache(name);
            else
            {
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
        internal static bool AllowNullValueControl<T>(bool allowNullValue, Func<T> setFunc, out T t)
        {
            t = setFunc.Invoke();
            return allowNullValue ? allowNullValue : t != null;
        }
    }
}