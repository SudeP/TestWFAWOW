using System;
using System.Runtime.Caching;
using System.Web.Caching;

namespace HybridServer
{
    internal sealed class OutputCacheHybridServerProvider : OutputCacheProvider
    {
        public OutputCacheHybridServerProvider()
        {

        }
        public override object Add(string key, object entry, DateTime utcExpiry)
        {
            SettingsJson settingsJson = Statics.settingsJsons.AddOrUpdate(
                key,
                _ => new SettingsJson(key, entry),
                (_, oldSettingsJson) => oldSettingsJson.CachedVary != null ? oldSettingsJson : oldSettingsJson.Update(key, entry));

            return settingsJson.CachedVary;
        }
        public override object Get(string key)
        {
            if (ProviderUtility.IsFirstPick(key))
            {
                SettingsJson settingsJson = Statics.settingsJsons.GetOrAdd(
                    key,
                    k => new SettingsJson(k, null));

                return settingsJson.CachedVary;
            }
            else
            {
                object outputCacheEntry = null;

                if (Statics.settingsJsons.TryGetValue(ProviderUtility.Impure2Pure(key), out SettingsJson settingsJson))
                    if (settingsJson.CacheSettings.TryGetValue(key, out CacheSettings cacheSettings) && cacheSettings.UtcExpiry > DateTime.UtcNow)
                        outputCacheEntry = cacheSettings.OutputCacheEntry;

                return outputCacheEntry;
            }
        }
        public override void Remove(string key)
        {
            if (!ProviderUtility.IsFirstPick(key) && Statics.settingsJsons.TryGetValue(ProviderUtility.Impure2Pure(key), out SettingsJson settingsJson))
                settingsJson.CacheSettings.TryRemove(key, out _);
        }
        public override void Set(string key, object entry, DateTime utcExpiry)
        {
            if (Statics.settingsJsons.TryGetValue(ProviderUtility.Impure2Pure(key), out SettingsJson settingsJson))
            {
                settingsJson.QueueTasker.Add(() =>
                {
                    if (settingsJson.CacheSettings.TryGetValue(key, out CacheSettings cacheSettings) && cacheSettings.UtcExpiry > DateTime.UtcNow)
                        return;

                    cacheSettings = settingsJson.CacheSettings.AddOrUpdate(
                        key,
                        _ => new CacheSettings(key, entry, utcExpiry, settingsJson),
                        (_, oldCacheSettings) => oldCacheSettings.UtcExpiry < DateTime.UtcNow ? oldCacheSettings : oldCacheSettings.Update(key, entry, utcExpiry, settingsJson));

                    IOUtility.Serialize(cacheSettings.PhysicalPath, cacheSettings.OutputCacheEntry);
                });
            }
        }
    }
}