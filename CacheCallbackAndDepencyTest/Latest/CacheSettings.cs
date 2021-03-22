using System;

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

            Uri = Statics.Request.Url;
            HttpMethod = Statics.Request.HttpMethod;

            settingsJson = parentSettingsJson;
            PhysicalPath = IOUtility.PathCombine(settingsJson.PhysicalPath, IOUtility.GuidFileName(Guid));

            return this;
        }
        [NonSerialized]
        private SettingsJson settingsJson;
        internal string PhysicalPath { get; private set; }
        internal Uri Uri { get; private set; }
        internal string HttpMethod { get; private set; }
        internal object OutputCacheEntry { get; private set; }
        internal string Key { get; private set; }
        internal DateTime UtcExpiry { get; private set; }
        internal Guid Guid { get; private set; }
    }
}