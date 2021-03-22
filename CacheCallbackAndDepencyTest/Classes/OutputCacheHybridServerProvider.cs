using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Routing;

internal sealed class OutputCacheHybridServerProvider : OutputCacheProvider
{
    private readonly MemoryCache mcd;
    private static readonly ConcurrentDictionary<string, SettingsJson> settingsJsons = new ConcurrentDictionary<string, SettingsJson>();
    internal OutputCacheHybridServerProvider()
    {
        mcd = CacheExtended.CreateSpecial("perDSecond", 0);
    }
    public override object Add(string key, object entry, DateTime utcExpiry)
    {
        var old = mcd.Get(key);
        if (old != null)
            return old;
        else
        {
            settingsJsons.TryAdd(key, new SettingsJson(key, entry));
            mcd.Set(key, entry, utcExpiry);
            return entry;
        }
    }

    public override object Get(string key)
    {
        var obj = mcd.Get(key);
        return obj;
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
internal class SettingsJson
{
    internal ConcurrentDictionary<string, CacheSettings> CacheSettings { get; set; }
    internal SettingsJson(string key, object cachedVary)
    {
        QueueTasker = new QueueTasker();
        CacheSettings = new ConcurrentDictionary<string, CacheSettings>();
        Key = key;
        CachedVary = cachedVary;
    }
    private const string cacheRegionName = "caches";
    internal QueueTasker QueueTasker { get; }
    internal string Key { get; }
    internal object CachedVary { get; }
    internal string GetRoute()
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

        return Path.Combine(cacheRegionName, controllerString, actionString);
    }
}
internal class CacheSettings
{
    internal CacheSettings(string filePath, DateTime utcExpiry)
    {
        HttpRequest request = HttpContext.Current.Request;
        OriginalString = request.Url.OriginalString;
        RawUrl = request.RawUrl;
        FilePath = filePath;
        UtcExpiry = utcExpiry;
    }
    internal string OriginalString { get; }
    internal string RawUrl { get; }
    internal string FilePath { get; }
    internal DateTime UtcExpiry { get; }
}
public static class CacheExtended
{
    public static MemoryCache CreateSpecial(string name, byte periodSecond)
    {
        MemoryCache memoryCache = null;

        if (periodSecond < 1)
            memoryCache = new MemoryCache(name);
        else
        {
            Assembly assembly = typeof(CacheItemPolicy).Assembly;

            Type type = assembly.GetType("System.Runtime.Caching.CacheExpires");

            if (type != null)
            {
                FieldInfo field = type.GetField("_tsPerBucket", BindingFlags.Static | BindingFlags.NonPublic);

                if (field != null && field.FieldType == typeof(TimeSpan))
                {
                    TimeSpan originalValue = (TimeSpan)field.GetValue(null);

                    field.SetValue(null, TimeSpan.FromSeconds(periodSecond));

                    memoryCache = new MemoryCache(name);

                    field.SetValue(null, originalValue);
                }
            }
        }
        return memoryCache;
    }
    public static T GetOrSet<T>(
        this MemoryCache memoryCache,
        string key,
        DateTime absoluteExpiration,
        Func<T> setFunc,
        bool allowNullValue) where T : class
    {
        if (memoryCache is null)
            return default;

        T t = default;

        if (memoryCache.Contains(key))
            t = (T)memoryCache.Get(key);
        else if (AllowNullValueControl(allowNullValue, setFunc, out t))
            memoryCache.Set(new CacheItem(key, t), new CacheItemPolicy
            {
                AbsoluteExpiration = absoluteExpiration,
                SlidingExpiration = new TimeSpan()
            });
        return t;
    }
    public static T Updatable<T>(
        this MemoryCache memoryCache,
        string key,
        DateTime absoluteExpiration,
        bool allowNullValue,
        Func<T> setFunc)
    {
        if (memoryCache is null)
            return default;

        T t = default;

        if (memoryCache.Contains(key))
            t = (T)memoryCache.Get(key);
        else if (AllowNullValueControl(allowNullValue, setFunc, out t))
            memoryCache.Set(new CacheItem(key, t), new CacheItemPolicy
            {
                AbsoluteExpiration = absoluteExpiration,
                SlidingExpiration = new TimeSpan(),
                UpdateCallback = args =>
                {
                    if (args.RemovedReason == CacheEntryRemovedReason.Expired && AllowNullValueControl(allowNullValue, setFunc, out T newValue))
                        args.UpdatedCacheItem = new CacheItem(key, newValue);
                }
            });

        return t;
    }
    internal static bool AllowNullValueControl<T>(bool allowNullValue, Func<T> setFunc, out T t)
    {
        t = setFunc.Invoke();
        return allowNullValue ? allowNullValue : t != null;
    }
}
internal class QueueTasker
{
    private readonly LimitedConcurrencyLevelTaskScheduler lcts;
    private readonly List<Task> tasks;
    private readonly TaskFactory factory;
    private readonly CancellationTokenSource cts;
#pragma warning disable
    private readonly object lockObj;
#pragma warning restore
    internal QueueTasker()
    {
        lcts = new LimitedConcurrencyLevelTaskScheduler(1);
        tasks = new List<Task>();
        factory = new TaskFactory(lcts);
        cts = new CancellationTokenSource();
        lockObj = new object();
    }
    internal Task Add(Action action)
    {
        var task = factory.StartNew(action, cts.Token);
        tasks.Add(task);
        return task;
    }
}
internal class LimitedConcurrencyLevelTaskScheduler : TaskScheduler
{
    [ThreadStatic]
    private static bool _currentThreadIsProcessingItems;
    private readonly LinkedList<Task> _tasks = new LinkedList<Task>();
    private readonly int _maxDegreeOfParallelism;
    private int _delegatesQueuedOrRunning = 0;
    internal LimitedConcurrencyLevelTaskScheduler(int maxDegreeOfParallelism)
    {
        if (maxDegreeOfParallelism < 1) throw new ArgumentOutOfRangeException("maxDegreeOfParallelism");
        _maxDegreeOfParallelism = maxDegreeOfParallelism;
    }
    protected sealed override void QueueTask(Task task)
    {
        lock (_tasks)
        {
            _tasks.AddLast(task);
            if (_delegatesQueuedOrRunning < _maxDegreeOfParallelism)
            {
                ++_delegatesQueuedOrRunning;
                NotifyThreadPoolOfPendingWork();
            }
        }
    }
    internal void NotifyThreadPoolOfPendingWork()
    {
        ThreadPool.UnsafeQueueUserWorkItem(_ =>
        {
            _currentThreadIsProcessingItems = true;
            try
            {
                while (true)
                {
                    Task item;
                    lock (_tasks)
                    {
                        if (_tasks.Count == 0)
                        {
                            --_delegatesQueuedOrRunning;
                            break;
                        }
                        item = _tasks.First.Value;
                        _tasks.RemoveFirst();
                    }
                    TryExecuteTask(item);
                }
            }
            finally { _currentThreadIsProcessingItems = false; }
        }, null);
    }
    protected sealed override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
    {
        if (!_currentThreadIsProcessingItems) return false;
        if (taskWasPreviouslyQueued)
            if (TryDequeue(task))
                return TryExecuteTask(task);
            else
                return false;
        else
            return TryExecuteTask(task);
    }
    protected sealed override bool TryDequeue(Task task)
    {
        lock (_tasks) return _tasks.Remove(task);
    }
    public sealed override int MaximumConcurrencyLevel { get { return _maxDegreeOfParallelism; } }
    protected sealed override IEnumerable<Task> GetScheduledTasks()
    {
        bool lockTaken = false;
        try
        {
            Monitor.TryEnter(_tasks, ref lockTaken);
            if (lockTaken) return _tasks;
            else throw new NotSupportedException();
        }
        finally
        {
            if (lockTaken) Monitor.Exit(_tasks);
        }
    }
}