using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Caching;
using System.Web;
using System.Web.Caching;

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
    private string key => HttpContext.Current.Request.RawUrl;
    private OutputCacheProvider provider => OutputCache.Providers[OutputCache.DefaultProviderName];
    void OnEnter(object sender, EventArgs e)
    {
        HttpApplication app = (HttpApplication)sender;
        string data = (string)provider.Get(key);
        if (data != null)
        {
            app.Response.Write(data);
            app.CompleteRequest();
        }
        var v1 = app.Context.Handler;
        var v2 = app.Context.CurrentHandler;
        var v3 = app.Context.PreviousHandler;
    }
    void OnLeave(object sender, EventArgs e)
    {
        var app = (HttpApplication)sender;


        app.Context.Handler.ProcessRequest(app.Context);

        System.IO.Stream ResponseStream = app.Response.OutputStream;
        byte[] ResponseStreamArray = new byte[ResponseStream.Length];

        Int32 strRead2 = ResponseStream.Read(ResponseStreamArray, 0, ResponseStreamArray.Length);

        String ResponseBody = System.Text.Encoding.Default.GetString(ResponseStreamArray);


        string data = (string)provider.Get(key);
        if (data == null)
        {
            
        }
    }
}