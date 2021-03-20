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
    }
    void OnLeave(object sender, EventArgs e)
    {
        var app = (HttpApplication)sender;

        var response = app.Response;


        var httpRawResponse = response.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(pi => pi.Name == "GetSnapshot").ToList().FirstOrDefault().Invoke(response, null);

        var buffersT = httpRawResponse.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(pi => pi.Name == "Buffers").ToList().FirstOrDefault().GetValue(httpRawResponse);


        List<ResponseElement> responseElements = null;
        ArrayList buffers = (ArrayList)buffersT;
        var count = (buffers != null) ? buffers.Count : 0;
        for (int i = 0; i < count; i++)
        {
            if (responseElements == null)
            {
                responseElements = new List<ResponseElement>(count);
            }
            var elem = buffers[i];
            if (elem is HttpFileResponseElement)
            {
                HttpFileResponseElement fileElement = elem as HttpFileResponseElement;
                responseElements.Add(new FileResponseElement(fileElement.FileName, fileElement.Offset, elem.GetSize()));
            }
            else if (elem is HttpSubstBlockResponseElement)
            {
                HttpSubstBlockResponseElement substElement = elem as HttpSubstBlockResponseElement;
                responseElements.Add(new SubstitutionResponseElement(substElement.Callback));
            }
            else
            {
                byte[] b = elem.GetBytes();
                long length = (b != null) ? b.Length : 0;
                responseElements.Add(new MemoryResponseElement(b, length));
            }
        }




        string data = (string)provider.Get(key);
        if (data == null)
        {
            var o = app.Response.Output;
            if (o != null)
            {
                var ot = o.GetType();
                if (ot != null)
                {
                    var cbp = ot.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance).Where(pi => pi.Name == "CharBuffer").ToList().FirstOrDefault();
                    if (cbp != null)
                    {
                        var cb = cbp.GetValue(o);
                        if (cb != null)
                        {
                            char[] vs = (char[])cb;
                            if (vs != null)
                            {
                                string htm = new string(vs);
                            }
                        }
                    }
                }
            }
        }
    }
}