using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Web.Routing;

namespace HybridServer
{
    [Serializable]
    internal class SettingsJson
    {
        internal SettingsJson(string key, object cachedVary) => Update(key, cachedVary);
        internal SettingsJson Update(string key, object cachedVary)
        {
            if (QueueTasker != null)
                GC.SuppressFinalize(QueueTasker);
            QueueTasker = new QueueTasker();

            string controllerFileName = string.Empty;
            string actionFileName = string.Empty;

            try
            {
                RouteValueDictionary rvd = Statics.RequestContext.RouteData.Values;

                controllerFileName = (string)rvd["controller"];
                actionFileName = (string)rvd["action"];
            }
            catch (Exception exception)
            {
#if DEBUG
                Trace.Fail(exception.ToString());
#endif
            }

            controllerFileName ??= Statics.defaultControllerFolderName;
            actionFileName ??= Statics.defaultActionFolderName;

            RootPath = IOUtility.PathMap(Statics.defaultCacheRegionName, controllerFileName, actionFileName);

            if (!Directory.Exists(RootPath))
                Directory.CreateDirectory(RootPath);

            PhysicalPath = IOUtility.PathCombine(RootPath, Statics.defaultSettingsJsonFileName);

            SettingsJson settingsJson = IOUtility.Deserialize<SettingsJson>(PhysicalPath);

            if (settingsJson != null)
            {
                CacheSettings = settingsJson.CacheSettings;
                Key = settingsJson.Key;
                CachedVary = settingsJson.CachedVary;
                Guid = settingsJson.Guid;
            }
            else
            {
                CacheSettings = new ConcurrentDictionary<string, CacheSettings>();
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
        internal CacheSettings AddOrUpdate(string key, Func<string, CacheSettings> addValueFactory, Func<string, CacheSettings, CacheSettings> updateValueFactory)
        {
            isChange = true;
            return CacheSettings.AddOrUpdate(key, addValueFactory, updateValueFactory);
        }
        internal bool TryRemove(string key)
        {
            isChange = true;
            return CacheSettings.TryRemove(key, out _);
        }
        [NonSerialized]
        internal bool isChange;
        [NonSerialized]
        internal QueueTasker QueueTasker;
        internal string RootPath { get; private set; }
        internal string PhysicalPath { get; private set; }
        internal ConcurrentDictionary<string, CacheSettings> CacheSettings { get; set; }
        internal string Key { get; private set; }
        internal object CachedVary { get; private set; }
        internal Guid Guid { get; private set; }
    }
}