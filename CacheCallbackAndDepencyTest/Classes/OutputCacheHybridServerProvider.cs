using System;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using System.Web;
using System.Web.Caching;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Collections.Specialized;

public class OutputCacheHybridServerProvider : OutputCacheProvider
{
    readonly MemoryCache mc = MemoryCache.Default;
    public override object Add(string key, object entry, DateTime utcExpiry)
    {
        Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
        mc.Add(key, entry, utcExpiry);
        return entry;
    }
    public override object Get(string key)
    {
        Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
        var obj = mc.Get(key);
        return obj;
    }
    public override void Remove(string key)
    {
        Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
        mc.Remove(key);
    }
    public override void Set(string key, object entry, DateTime utcExpiry)
    {
        Debug.WriteLine(MethodBase.GetCurrentMethod().GetFullName());
        mc.Set(key, entry, utcExpiry);
    }
}
public class CustOutputCacheModule : IHttpModule
{
    public void Dispose()
    {
    }
    public void Init(HttpApplication context)
    {
        context.ResolveRequestCache += new EventHandler(OnEnter);
        context.UpdateRequestCache += new EventHandler(OnLeave);
    }
    private string Key => HttpContext.Current.Request.RawUrl;
    private OutputCacheProvider provider => OutputCache.Providers[OutputCache.DefaultProviderName];
    void OnEnter(object httpApplication, EventArgs eventArgs)
    {
        HttpApplication app = (HttpApplication)httpApplication;
        HttpContext context = app.Context;
        HttpResponse response = app.Response;
        HttpRequest request = app.Request;
        Type trp = response.GetType();

        BindingFlags bf = BindingFlags.CreateInstance
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

        var buffer = provider.Get(Key);
        if (buffer != null)
        {
            trp.GetMethod("SetResponseBuffers", bf).Invoke(response, new object[] { buffer });

            trp.GetField("_suppressContent", bf).SetValue(response, !(request.HttpMethod != "HEAD"));

            app.CompleteRequest();
        }
    }
    void OnLeave(object httpApplication, EventArgs eventArgs)
    {
        HttpApplication app = (HttpApplication)httpApplication;
        HttpContext context = app.Context;
        HttpResponse response = app.Response;

        var httpRawResponse = response.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(pi => pi.Name == "GetSnapshot").ToList().FirstOrDefault().Invoke(response, null);

        var buffersT = httpRawResponse.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(pi => pi.Name == "Buffers").ToList().FirstOrDefault().GetValue(httpRawResponse);

        ArrayList buffers = (ArrayList)buffersT;

        provider.Set(Key, buffers, DateTime.MaxValue);

    }
}