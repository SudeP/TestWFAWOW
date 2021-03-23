using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;

namespace HybridServer
{
    internal static class Statics
    {
        internal static Task Collector;
        internal const int oneMilliSecond = 1000;
        internal const int oneMinute = 60 * oneMilliSecond;
        internal const string defaultFileExtesion = ".hsf";
        internal const string defaultCacheRegionName = "__cacheFiles";
        internal const string defaultSettingsJsonFileName = "settings" + defaultFileExtesion;
        internal const string defaultControllerFolderName = "__unNamedController";
        internal const string defaultActionFolderName = "__unNamedAction";
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
        internal static HttpContext Context => HttpContext.Current;
        internal static HttpApplication Application => Context.ApplicationInstance;
        internal static HttpServerUtility Server => Context.Server;
        internal static HttpResponse Response => Context.Response;
        internal static HttpRequest Request => Context.Request;
        internal static RequestContext RequestContext => Request.RequestContext;
    }
}