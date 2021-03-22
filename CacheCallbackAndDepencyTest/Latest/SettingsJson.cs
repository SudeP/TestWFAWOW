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
                Trace.Fail(exception.ToString());
            }

            controllerFileName ??= Statics.defaultControllerFolderName;
            actionFileName ??= Statics.defaultActionFolderName;

            PathRoot = IOUtility.PathMap(Statics.defaultCacheRegionName, controllerFileName, actionFileName);


            if (!Directory.Exists(PathRoot))
                Directory.CreateDirectory(PathRoot);

            PhysicalPath = IOUtility.PathCombine(PathRoot, Statics.defaultSettingsJsonFileName);

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
        internal string PathRoot { get; private set; }
        internal string PhysicalPath { get; private set; }
        internal ConcurrentDictionary<string, CacheSettings> CacheSettings { get; set; }
        internal QueueTasker QueueTasker { get; private set; }
        internal string Key { get; private set; }
        internal object CachedVary { get; private set; }
        internal Guid Guid { get; private set; }
    }
}