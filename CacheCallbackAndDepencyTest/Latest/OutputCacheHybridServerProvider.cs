using System;
using System.Runtime.Caching;
using System.Web.Caching;

namespace HybridServer
{
    internal sealed class OutputCacheHybridServerProvider : OutputCacheProvider
    {
        private readonly MemoryCache mcd;
        internal OutputCacheHybridServerProvider()
        {
            mcd = CacheExtended.CreateSpecial("perDSecond", 0);
        }
        public override object Add(string key, object entry, DateTime utcExpiry)
        {
            SettingsJson settingsJson = Statics.settingsJsons.AddOrUpdate(
                key,
                new SettingsJson(key, entry),
                (_, oldSettingsJson) => oldSettingsJson.CachedVary != null ? oldSettingsJson : new SettingsJson(key, entry));
            return settingsJson.CachedVary;
        }
        public override object Get(string key)
        {
            SettingsJson settingsJson = Statics.settingsJsons.GetOrAdd(
                key,
                k => new SettingsJson(k, null));

            return settingsJson.CachedVary;
        }
        public override void Remove(string key)
        {
            mcd.Remove(key);
        }
        public override void Set(string key, object entry, DateTime utcExpiry)
        {
            mcd.Set(key, entry, utcExpiry);
        }
    }
}