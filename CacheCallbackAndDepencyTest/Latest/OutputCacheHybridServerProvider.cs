using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace HybridServer
{
    internal sealed class OutputCacheHybridServerProvider : OutputCacheProvider
    {
        public OutputCacheHybridServerProvider()
        {
            if (Statics.Collector == null)
                Statics.Collector = Task.Factory.StartNew(() =>
                {
                    do
                    {
                        KeyValuePair<string, HSSettings>[] vs = Statics.HSSettings.ToArray();

                        for (int i = 0; i < vs.Length; i++)
                        {
                            if (vs[i].Value.isChange)
                            {
                                vs[i].Value.isChange = false;

                                IOUtility.Serialize(vs[i].Value.PhysicalPath, vs[i].Value);
                            }
                        }
                        Thread.Sleep(Statics.oneMinute);
                    } while (true);
                });
        }
        public override object Add(string key, object entry, DateTime utcExpiry)
        {
            HSSettings hSSettings = Statics.HSSettings.AddOrUpdate(
                key,
                _ => new HSSettings(key, entry),
                (_, oldHSSettings) => oldHSSettings.CachedVary != null ? oldHSSettings : oldHSSettings.Update(key, entry));

            return hSSettings.CachedVary;
        }
        public override object Get(string key)
        {
            if (ProviderUtility.IsFirstPick(key))
            {
                HSSettings hSSettings = Statics.HSSettings.GetOrAdd(
                    key,
                    k => new HSSettings(k, null));

                return hSSettings.CachedVary;
            }
            else
            {
                object outputCacheEntry = null;

                if (Statics.HSSettings.TryGetValue(ProviderUtility.Impure2Pure(key), out HSSettings hSSettings))
                    if (hSSettings.HSCache.TryGetValue(key, out HSCache hSCache))
                    {
                        if (hSCache.UtcExpiry > DateTime.UtcNow)
                            outputCacheEntry = hSCache.OutputCacheEntry;
                        else
                            ;
                        //ProviderUtility.UseSnapShot(hSCache.OutputCacheEntry);
                    }

                return outputCacheEntry;
            }
        }
        public override void Remove(string key)
        {
            if (!ProviderUtility.IsFirstPick(key) && Statics.HSSettings.TryGetValue(ProviderUtility.Impure2Pure(key), out HSSettings hSSettings))
                hSSettings.TryRemove(key);
        }
        public override void Set(string key, object entry, DateTime utcExpiry)
        {
            if (Statics.HSSettings.TryGetValue(ProviderUtility.Impure2Pure(key), out HSSettings hSSettings))
            {
                hSSettings.QueueTasker.Add(() =>
                {
                    if (hSSettings.HSCache.TryGetValue(key, out HSCache hSCache) && hSCache.UtcExpiry > DateTime.UtcNow)
                        return;

                    hSCache = hSSettings.AddOrUpdate(
                        key,
                        _ => new HSCache(key, entry, utcExpiry, hSSettings),
                        (_, oldHSCache) => oldHSCache.UtcExpiry < DateTime.UtcNow ? oldHSCache : oldHSCache.Update(key, entry, utcExpiry, hSSettings));

                    IOUtility.Serialize(hSCache.PhysicalPath, hSCache.OutputCacheEntry);
                });
            }
        }
    }
}