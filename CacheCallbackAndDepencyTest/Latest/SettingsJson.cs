using System;
using System.Collections.Concurrent;
using System.IO;
using System.Web;
using System.Web.Routing;

namespace HybridServer
{
    internal class SettingsJson
    {
        internal ConcurrentDictionary<string, CacheSettings> CacheSettings { get; set; }
        internal SettingsJson(string key, object cachedVary)
        {
            RouteValueDictionary rvd = HttpContext.Current.Request.RequestContext.RouteData.Values;

            object controller = rvd["controller"];
            string controllerString = "__unNamedController";
            if (controller != null)
                controllerString = controller.ToString();

            object action = rvd["action"];
            string actionString = "__unNamedAction";
            if (action != null)
                actionString = controller.ToString();

            pathRoot = HttpContext.Current.Server.MapPath(Path.Combine(Statics.cacheRegionName, controllerString, actionString));

            Directory.CreateDirectory(pathRoot);

            pathSettingJson = Path.Combine(pathRoot, Statics.settingsJsonFileName);

            if (File.Exists(pathSettingJson))
                File.Create(pathSettingJson);


            SettingsJson fileSettingsJson = 

            QueueTasker = new QueueTasker();
            CacheSettings = new ConcurrentDictionary<string, CacheSettings>();
            Key = key;
            CachedVary = cachedVary;
            if (CachedVary != null)
            {
                Guid = (Guid)CachedVary.GetType()
                    .GetProperty("CachedVaryId", Statics.bf)
                    .GetValue(CachedVary);
            }
            else
            {

            }
        }
        private readonly string pathRoot;
        private readonly string pathSettingJson;
        internal QueueTasker QueueTasker { get; }
        internal string Key { get; }
        internal object CachedVary { get; }
        internal Guid Guid { get; }
    }
}