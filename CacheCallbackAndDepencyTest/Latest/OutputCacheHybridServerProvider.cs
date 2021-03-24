using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace HybridServer
{
    internal sealed class OutputCacheHybridServerProvider : OutputCacheProvider
    {
        private HttpContext httpContext;
        public OutputCacheHybridServerProvider() => ProviderUtility.CollectorRun(Statics.oneMinute);
        public override object Add(string key, object entry, DateTime utcExpiry) => Statics.HSSettings.AddOrUpdate(
                key,
                _ => new HSSettings(httpContext, key, entry),
                (_, oldHSSettings) => oldHSSettings.CachedVary != null ? oldHSSettings : oldHSSettings.Update(httpContext, key, entry))
            .CachedVary;
        public override object Get(string key)
        {
            httpContext = HttpContext.Current;
            if (ProviderUtility.IsFirstPick(httpContext, key))
            {
                HSSettings hSSettings = Statics.HSSettings.GetOrAdd(
                    key,
                    k => new HSSettings(httpContext, k, null));

                return hSSettings.CachedVary;
            }
            else
            {
                object outputCacheEntry = null;

                if (
                    Statics.HSSettings.TryGetValue(ProviderUtility.Impure2Pure(httpContext, key), out HSSettings hSSettings)
                    &&
                    hSSettings.HSCache.TryGetValue(key, out HSCache hSCache))
                {
                    outputCacheEntry = hSCache.OutputCacheEntry;

                    if (hSCache.UtcExpiry < DateTime.UtcNow && !hSCache.IsReloding)
                    {
                        hSCache.IsReloding = true;
                        ProviderUtility
                            .InvokeRequest(
                                ReflectionUtility
                                    .Bind(
                                        httpContext,
                                        ProviderUtility.EmptyContext(httpContext.Request.Url)))
                                .ContinueWith((t) =>
                                {
                                    httpContext = t.Result;
                                    Set(key, ProviderUtility.GetSnapShot(httpContext), httpContext.Response.Cache.GetExpires());
                                });
                    }


                    // RUNTIME DAKI PROJEDEN RESULTEXECINTDEKİ PROCESS METHODUNDAN EXCEPTION VEREBILECEK YERLERI BUL
                    // RESPONSE METHODUYLA ALAKASINI BUL.
                    // ÇÖZ VE BİTSİN

                    //if (hSCache.UtcExpiry < DateTime.UtcNow && !hSCache.IsReloding)
                    //{
                    //    hSCache.IsReloding = true;
                    //    ProviderUtility
                    //        .InvokeRequest(
                    //            ProviderUtility.CloneContext(httpContext))
                    //        .ContinueWith((task) =>
                    //        {
                    //            httpContext = task.Result;
                    //            Set(key, ProviderUtility.GetSnapShot(httpContext), httpContext.Response.Cache.GetExpires());
                    //        });
                    //}
                }
                return outputCacheEntry;
            }
        }
        public override void Remove(string key)
        {
            if (!ProviderUtility.IsFirstPick(httpContext, key) && Statics.HSSettings.TryGetValue(ProviderUtility.Impure2Pure(httpContext, key), out HSSettings hSSettings))
                hSSettings.TryRemove(key);
        }
        public override void Set(string key, object entry, DateTime utcExpiry)
        {
            if (Statics.HSSettings.TryGetValue(ProviderUtility.Impure2Pure(httpContext, key), out HSSettings hSSettings))
            {
                hSSettings.QueueTasker.Add(() =>
                {
                    if (hSSettings.HSCache.TryGetValue(key, out HSCache hSCache) && hSCache.UtcExpiry > DateTime.UtcNow)
                        return;

                    hSCache = hSSettings.AddOrUpdate(
                        key,
                        _ => new HSCache(key, entry, utcExpiry, hSSettings),
                        (_, oldHSCache) => oldHSCache.UtcExpiry > DateTime.UtcNow ? oldHSCache : oldHSCache.Update(key, entry, utcExpiry, hSSettings));

                    IOUtility.Serialize(hSCache.PhysicalPath, hSCache.OutputCacheEntry);
                    hSCache.IsReloding = false;
                });
            }
        }
    }
}