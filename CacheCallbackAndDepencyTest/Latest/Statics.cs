using System.Collections.Concurrent;
using System.Reflection;

namespace HybridServer
{
    internal static class Statics
    {
        internal const string cacheRegionName = "caches";
        internal const string settingsJsonFileName = "settings.json";
        internal static readonly ConcurrentDictionary<string, SettingsJson> settingsJsons = new ConcurrentDictionary<string, SettingsJson>();
        internal static BindingFlags bf = BindingFlags.CreateInstance
            | BindingFlags.DeclaredOnly
            | BindingFlags.Default
            | BindingFlags.ExactBinding
            | BindingFlags.FlattenHierarchy
            | BindingFlags.GetField
            | BindingFlags.GetProperty
            | BindingFlags.IgnoreCase
            | BindingFlags.IgnoreReturn
            | BindingFlags.Instance
            | BindingFlags.InvokeMethod
            | BindingFlags.NonPublic
            | BindingFlags.OptionalParamBinding
            | BindingFlags.Public
            | BindingFlags.PutDispProperty
            | BindingFlags.PutRefDispProperty
            | BindingFlags.SetField
            | BindingFlags.SetProperty
            | BindingFlags.Static
            | BindingFlags.SuppressChangeType;
    }
}