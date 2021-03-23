using System;
using System.IO;

namespace HybridServer
{
    [Serializable]
    internal class CacheSettings
    {
        internal CacheSettings(string key, object outputCacheEntry, DateTime utcExpiry, SettingsJson parentSettingsJson) => Update(key, outputCacheEntry, utcExpiry, parentSettingsJson);
        internal CacheSettings Update(string key, object outputCacheEntry, DateTime utcExpiry, SettingsJson parentSettingsJson)
        {
            Key = key;
            UtcExpiry = utcExpiry;
            OutputCacheEntry = outputCacheEntry;

            Guid = (Guid)OutputCacheEntry.GetType()
                .GetField("_cachedVaryId", Statics.bf)
                .GetValue(OutputCacheEntry);

            settingsJson = parentSettingsJson;

            RootPath = IOUtility.PathCombine(settingsJson.RootPath, Guid.ToString());

            if (!Directory.Exists(RootPath))
                Directory.CreateDirectory(RootPath);

            PhysicalPath = IOUtility.PathCombine(RootPath, IOUtility.CreateFileName());

            return this;
        }
        [NonSerialized]
        private SettingsJson settingsJson;
        internal string RootPath { get; private set; }
        internal string PhysicalPath { get; private set; }
        internal object OutputCacheEntry { get; private set; }
        internal string Key { get; private set; }
        internal DateTime UtcExpiry { get; private set; }
        internal Guid Guid { get; private set; }
    }
}