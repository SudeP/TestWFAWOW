using System;
using System.Collections.Concurrent;
using System.IO;
using System.Web;

namespace HybridServer
{
    [Serializable]
    internal class HSSettings
    {
        internal HSSettings(HttpContext httpContext, string key, object cachedVary) => Update(httpContext, key, cachedVary);
        internal HSSettings Update(HttpContext httpContext, string key, object cachedVary)
        {
            if (QueueTasker != null)
                GC.SuppressFinalize(QueueTasker);
            QueueTasker = new QueueTasker();

            RootPath = IOUtility.PathMap(httpContext, Statics.defaultCacheRegion, IOUtility.KeyCrypte(key));

            if (!Directory.Exists(RootPath))
                Directory.CreateDirectory(RootPath);

            PhysicalPath = IOUtility.PathCombine(RootPath, Statics.defaultSettingsFileName);


            HSSettings hSDettings = IOUtility.Deserialize<HSSettings>(PhysicalPath);

            if (hSDettings != null)
            {
                HSCache = hSDettings.HSCache;
                Key = hSDettings.Key;
                CachedVary = hSDettings.CachedVary;
                Guid = hSDettings.Guid;
            }
            else
            {
                HSCache = new ConcurrentDictionary<string, HSCache>();
                Key = key;
                CachedVary = cachedVary;
                if (CachedVary != null)
                {
                    Guid = (Guid)CachedVary.GetType()
                        .GetProperty("CachedVaryId", Statics.bf)
                        .GetValue(CachedVary);

                    IOUtility.Serialize(PhysicalPath, this);
                }
            }
            return this;
        }
        internal HSCache AddOrUpdate(string key, Func<string, HSCache> addValueFactory, Func<string, HSCache, HSCache> updateValueFactory)
        {
            isChange = true;
            return HSCache.AddOrUpdate(key, addValueFactory, updateValueFactory);
        }
        internal bool TryRemove(string key)
        {
            isChange = true;
            return HSCache.TryRemove(key, out _);
        }
        [NonSerialized]
        internal bool isChange;
        [NonSerialized]
        internal QueueTasker QueueTasker;
        internal string RootPath { get; private set; }
        internal string PhysicalPath { get; private set; }
        internal ConcurrentDictionary<string, HSCache> HSCache { get; set; }
        internal string Key { get; private set; }
        internal object CachedVary { get; private set; }
        internal Guid Guid { get; private set; }
    }
}