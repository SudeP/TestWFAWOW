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
    const int MAX_POST_KEY_LENGTH = 15000;
    const string NULL_VARYBY_VALUE = "+n+";
    const string ERROR_VARYBY_VALUE = "+e+";
    internal const string TAG_OUTPUTCACHE = "OutputCache";
    const string OUTPUTCACHE_KEYPREFIX_POST = "a1";
    const string OUTPUTCACHE_KEYPREFIX_GET = "f2";
    const string IDENTITY = "identity";
    const string ASTERISK = "*";
    string _key;
    private string key => HttpContext.Current.Request.RawUrl;
    private OutputCacheProvider provider => OutputCache.Providers[OutputCache.DefaultProviderName];
    void OnEnter(object sender, EventArgs e)
    {
        //var v1 = typeof(OutputCache);
        //var methods = v1.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        //HttpApplication app = (HttpApplication)sender;
        //string data = (string)provider.Get(key);
        //if (data != null)
        //{
        //    app.Response.Write(data);
        //    app.CompleteRequest();
        //}
    }
    void OnLeave(object httpApplication, EventArgs eventArgs)
    {
        HttpApplication ha;
        HttpContext context;
        bool cacheable;
        object cachedVary;
        HttpCachePolicy cache;
        object settings;
        string keyRawResponse;
        string[] varyByContentEncodings;
        string[] varyByHeaders;
        string[] varyByParams;
        bool varyByAllParams;
        HttpRequest request;
        HttpResponse response;
        int i, n;
        bool cacheAuthorizedPage;

        ha = (HttpApplication)httpApplication;
        context = ha.Context;
        response = ha.Response;
        request = ha.Request;
        cache = response.Cache;
        cacheable = false;

        Type t = GetType();
        Type tap = ha.GetType();
        Type tcx = context.GetType();
        Type trp = response.GetType();
        Type trq = request.GetType();
        Type tch = cache.GetType();

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

        do
        {
            var trpHasCachePolicy = trp.GetProperty("HasCachePolicy", bf);
            if (trpHasCachePolicy != null)
            {
                bool hasCachePolicy = (bool)trpHasCachePolicy.GetValue(response);

                if (!hasCachePolicy)
                    break;
            }

            if (!cache.IsModified())
                break;

            if (response.StatusCode != 200)
                break;


            if (request.HttpMethod != "GET" && request.HttpMethod != "POST")
                break;

            var trpIsBuffered = trp.GetMethod("IsBuffered", bf);
            if (trpIsBuffered != null)
            {
                bool isBuffered = (bool)trpIsBuffered.Invoke(response, null);

                if (!isBuffered)
                    break;
            }

            var tcxRequestRequiresAuthorization = tcx.GetMethod("RequestRequiresAuthorization", bf);
            cacheAuthorizedPage = false;
            if (tcxRequestRequiresAuthorization != null)
            {
                bool requestRequiresAuthorization = (bool)tcxRequestRequiresAuthorization.Invoke(context, null);

                if (cache.GetCacheability() == HttpCacheability.Public && requestRequiresAuthorization)
                {
                    cache.SetCacheability(HttpCacheability.Private);
                    cacheAuthorizedPage = true;
                }
            }

            if (cache.GetCacheability() != HttpCacheability.Public &&
                    cache.GetCacheability() != HttpCacheability.ServerAndPrivate &&
                    cache.GetCacheability() != HttpCacheability.ServerAndNoCache &&
                    !cacheAuthorizedPage)
                break;

            if (cache.GetNoServerCaching())
                break;

            var trpContainsNonShareableCookies = trp.GetMethod("ContainsNonShareableCookies", bf);
            if (trpContainsNonShareableCookies != null)
            {
                bool containsNonShareableCookies = (bool)trpContainsNonShareableCookies.Invoke(response, null);

                if (!containsNonShareableCookies)
                    break;
            }

            var tchHasExpirationPolicy = tch.GetMethod("HasExpirationPolicy", bf);
            var tchHasValidationPolicy = tch.GetMethod("HasValidationPolicy", bf);
            if (tchHasExpirationPolicy != null && tchHasValidationPolicy != null)
            {
                bool HasExpirationPolicy = (bool)tchHasExpirationPolicy.Invoke(cache, null);
                bool HasValidationPolicy = (bool)tchHasValidationPolicy.Invoke(cache, null);

                if (!HasExpirationPolicy && !HasValidationPolicy)
                    break;
            }

            var tvbh = cache.VaryByHeaders.GetType();

            var tvbhGetVaryByUnspecifiedParameters = tvbh.GetMethod("GetVaryByUnspecifiedParameters", bf);
            if (tvbhGetVaryByUnspecifiedParameters != null)
            {
                bool getVaryByUnspecifiedParameters = (bool)tvbhGetVaryByUnspecifiedParameters.Invoke(cache.VaryByHeaders, null);

                if (getVaryByUnspecifiedParameters)
                    break;
            }

            var tvbp = cache.VaryByParams.GetType();

            var tvbpAcceptsParams = tvbp.GetMethod("AcceptsParams", bf);
            if (tvbpAcceptsParams != null)
            {
                bool acceptsParams = (bool)tvbpAcceptsParams.Invoke(cache.VaryByParams, null);

                if (!acceptsParams && (request.HttpMethod == "POST" || request.Url.Query.Length > 0))
                    break;
            }

            var tvbce = cache.VaryByContentEncodings.GetType();

            var tvbceIsModified = tvbce.GetMethod("IsModified", bf);
            var tvbceIsCacheableEncoding = tvbce.GetMethod("IsCacheableEncoding", bf);
            var trpGetHttpHeaderContentEncoding = trp.GetMethod("GetHttpHeaderContentEncoding", bf);
            if (tvbceIsModified != null && tvbceIsCacheableEncoding != null && trpGetHttpHeaderContentEncoding != null)
            {
                bool isModified = (bool)tvbceIsModified.Invoke(cache.VaryByContentEncodings, null);
                bool isCacheableEncoding = (bool)tvbceIsCacheableEncoding.Invoke(cache.VaryByContentEncodings, new object[] { trpGetHttpHeaderContentEncoding.Invoke(response, null) });

                if (isModified && !isCacheableEncoding)
                    break;
            }

            cacheable = true;
        } while (false);

        if (!cacheable)
            return;

        var tchGetCurrentSettings = tch.GetMethod("GetCurrentSettings", bf);
        if (tchGetCurrentSettings == null)
            return;

        settings = tchGetCurrentSettings.Invoke(cache, new object[] { response });

        Type tst = settings.GetType();

        var tstVaryByContentEncodings = tst.GetProperty("VaryByContentEncodings", bf);
        if (tstVaryByContentEncodings == null)
            return;

        varyByContentEncodings = (string[])tstVaryByContentEncodings.GetValue(settings);

        var tstVaryByHeaders = tst.GetProperty("VaryByHeaders", bf);
        if (tstVaryByHeaders == null)
            return;
        varyByHeaders = (string[])tstVaryByHeaders.GetValue(settings);

        var tstIgnoreParams = tst.GetProperty("IgnoreParams", bf);
        if (tstIgnoreParams == null)
            return;
        bool ignoreParams = (bool)tstIgnoreParams.GetValue(settings);

        if (ignoreParams)
            varyByParams = null;
        else
        {
            var tstVaryByParams = tst.GetProperty("VaryByParams", bf);
            if (tstVaryByParams == null)
                return;

            varyByParams = (string[])tstVaryByParams.GetValue(settings);
        }

        var tCreateOutputCachedItemKey = t.GetMethods(bf).FirstOrDefault(m => m.Name == "CreateOutputCachedItemKey" && m.GetParameters().Length == 2);
        if (tCreateOutputCachedItemKey is null)
            return;

        string cocik(params object[] vs) => (string)tCreateOutputCachedItemKey.Invoke(this, vs);

        if (_key == null)
            _key = cocik(context, null);

        var tstVaryByCustom = tst.GetProperty("VaryByCustom", bf);
        if (tstVaryByCustom == null)
            return;
        var varyByCustom = (string)tstVaryByCustom.GetValue(settings);

        if (varyByContentEncodings == null && varyByHeaders == null && varyByParams == null && varyByCustom == null)
        {
            keyRawResponse = _key;
            cachedVary = null;
        }
        else
        {
            if (varyByHeaders != null)
            {
                for (i = 0, n = varyByHeaders.Length; i < n; i++)
                {
                    varyByHeaders[i] = "HTTP_" + CultureInfo.InvariantCulture.TextInfo.ToUpper(
                            varyByHeaders[i].Replace('-', '_'));
                }
            }

            varyByAllParams = false;
            if (varyByParams != null)
            {
                varyByAllParams = (varyByParams.Length == 1 && varyByParams[0] == ASTERISK);
                if (varyByAllParams)
                {
                    varyByParams = null;
                }
                else
                {
                    for (i = 0, n = varyByParams.Length; i < n; i++)
                    {
                        varyByParams[i] = CultureInfo.InvariantCulture.TextInfo.ToLower(varyByParams[i]);
                    }
                }
            }

            var tcv = Type.GetType("System.Web.Caching.CachedVary");

            cachedVary = Activator.CreateInstance(tcv, varyByContentEncodings, varyByHeaders, varyByParams, varyByAllParams, varyByCustom);

            keyRawResponse = cocik(context, cachedVary);
            if (keyRawResponse == null)
                return;

            var trpIsBuffered = trp.GetMethod("IsBuffered", bf);
            if (trpIsBuffered == null)
                return;
            bool isBuffered = (bool)trpIsBuffered.Invoke(response, null);

            if (!isBuffered)
                return;
        }

        DateTime utcExpires = Cache.NoAbsoluteExpiration;
        TimeSpan slidingDelta = Cache.NoSlidingExpiration;


        var tstSlidingExpiration = tst.GetProperty("SlidingExpiration", bf);
        if (tstSlidingExpiration == null)
            return;

        bool slidingExpiration = (bool)tstSlidingExpiration.GetValue(settings);

        if (slidingExpiration)
        {
            var tstSlidingDelta = tst.GetProperty("SlidingDelta", bf);
            if (tstSlidingDelta == null)
                return;

            slidingDelta = (TimeSpan)tstSlidingDelta.GetValue(settings);
        }
        else
        {
            var tstIsMaxAgeSet = tst.GetProperty("IsMaxAgeSet", bf);
            if (tstIsMaxAgeSet == null)
                return;

            bool isMaxAgeSet = (bool)tstIsMaxAgeSet.GetValue(settings);

            if (isMaxAgeSet)
            {
                var tstUtcTimestampCreated = tst.GetProperty("UtcTimestampCreated", bf);
                if (tstUtcTimestampCreated == null)
                    return;

                DateTime utcTimestampCreated = (DateTime)tstUtcTimestampCreated.GetValue(settings);

                var tstUtcTimestamp = tst.GetProperty("UtcTimestamp", bf);
                if (tstUtcTimestamp == null)
                    return;

                DateTime utcTimestamp = (DateTime)tstUtcTimestamp.GetValue(settings);

                var tstMaxAge = tst.GetProperty("MaxAge", bf);
                if (tstMaxAge == null)
                    return;

                TimeSpan maxAge = (TimeSpan)tstMaxAge.GetValue(settings);


                utcExpires = (utcTimestampCreated != DateTime.MinValue) ? utcTimestampCreated : utcTimestamp + maxAge;
            }
            else
            {
                var tstIsExpiresSet = tst.GetProperty("IsExpiresSet", bf);
                if (tstIsExpiresSet == null)
                    return;

                bool isExpiresSet = (bool)tstIsExpiresSet.GetValue(settings);

                if (isExpiresSet)
                {
                    var tstUtcExpires = tst.GetProperty("UtcExpires", bf);
                    if (tstUtcExpires == null)
                        return;

                    utcExpires = (DateTime)tstUtcExpires.GetValue(settings);
                }
            }
        }

        if (utcExpires > DateTime.UtcNow)
        {

            object httpRawResponse = response.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(pi => pi.Name == "GetSnapshot").ToList().FirstOrDefault().Invoke(response, null);

            var trpSetupKernelCaching = trp.GetMethod("SetupKernelCaching", bf);
            if (trpSetupKernelCaching == null)
                return;

            string kernelCacheUrl = (string)trpSetupKernelCaching.Invoke(response, new object[] { null });

            var tcv = cachedVary.GetType();

            var tcvCachedVaryId = tcv.GetProperty("CachedVaryId", bf);
            if (tcvCachedVaryId == null)
                return;

            Guid cachedVaryId = (Guid)tcvCachedVaryId.GetValue(cachedVary);

            cachedVaryId = (cachedVary != null) ? cachedVaryId : Guid.Empty;

            var tcrr = Type.GetType("System.Web.Caching.CachedRawResponse");

            object cachedRawResponse = Activator.CreateInstance(tcrr, httpRawResponse, settings, kernelCacheUrl, cachedVaryId);

            CacheDependency dep = (CacheDependency)response.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(pi => pi.Name == "CreateCacheDependencyForResponse").ToList().FirstOrDefault().Invoke(response, null);

            try
            {
                var toc = Type.GetType("System.Web.Caching.OutputCache");

                response.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(pi => pi.Name == "InsertResponse").ToList().FirstOrDefault().Invoke(response, new object[] { _key, cachedVary, keyRawResponse, cachedRawResponse, dep, utcExpires, slidingDelta });
            }
            catch
            {
                if (dep != null)
                {
                    dep.Dispose();
                }
                throw;
            }
        }

        _key = null;

        //var httpRawResponse = response.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(pi => pi.Name == "GetSnapshot").ToList().FirstOrDefault().Invoke(response, null);


        //var buffersT = httpRawResponse.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(pi => pi.Name == "Buffers").ToList().FirstOrDefault().GetValue(httpRawResponse);


        //List<ResponseElement> responseElements = null;
        //ArrayList buffers = (ArrayList)buffersT;
        //var count = (buffers != null) ? buffers.Count : 0;
        //for (int i = 0; i < count; i++)
        //{
        //    if (responseElements == null)
        //    {
        //        responseElements = new List<ResponseElement>(count);
        //    }
        //    var elem = buffers[i];
        //    if (elem != null)
        //    {
        //        var et = elem.GetType();
        //        if (et != null)
        //        {
        //            var etn = et.Name;
        //            var em = et.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToList();
        //            var ef = et.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToList();
        //            var ep = et.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToList();
        //            if (etn == "HttpFileResponseElement")
        //            {
        //                var fileName = ep.FirstOrDefault(p => p.Name == "FileName");
        //                var fileNameVal = (string)fileName.GetValue(elem);
        //                var offset = ep.FirstOrDefault(p => p.Name == "Offset");
        //                var offsetVal = (long)offset.GetValue(elem);
        //                var getSize = em.FirstOrDefault(m => m.Name == "GetSize");

        //                responseElements.Add(new FileResponseElement(fileNameVal, offsetVal, (long)getSize.Invoke(elem, null)));
        //            }
        //            else if (etn == "HttpSubstBlockResponseElement")
        //            {

        //                var callback = ep.FirstOrDefault(p => p.Name == "Callback");
        //                var callbackVal = (HttpResponseSubstitutionCallback)callback.GetValue(elem);

        //                responseElements.Add(new SubstitutionResponseElement(callbackVal));
        //            }
        //            else
        //            {
        //                var getBytes = em.FirstOrDefault(m => m.Name == "System.Web.IHttpResponseElement.GetBytes");

        //                byte[] b = (byte[])getBytes.Invoke(elem, null);
        //                long length = (b != null) ? b.Length : 0;
        //                responseElements.Add(new MemoryResponseElement(b, length));
        //            }
        //        }
        //    }
        //}

        ////var httpRawResponse = response.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(pi => pi.Name == "UseSnapshot").ToList().FirstOrDefault().Invoke(response, null);
        ////var httpVerb = request.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(pi => pi.Name == "HttpVerb").ToList().FirstOrDefault().GetValue(request);
        ////int toInt = (int)httpVerb;
        ////var sendBody = toInt != 4;




        //string data = (string)provider.Get(key);
        //if (data == null)
        //{
        //    var o = app.Response.Output;
        //    if (o != null)
        //    {
        //        var ot = o.GetType();
        //        if (ot != null)
        //        {
        //            var cbp = ot.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance).Where(pi => pi.Name == "CharBuffer").ToList().FirstOrDefault();
        //            if (cbp != null)
        //            {
        //                var cb = cbp.GetValue(o);
        //                if (cb != null)
        //                {
        //                    char[] vs = (char[])cb;
        //                    if (vs != null)
        //                    {
        //                        string htm = new string(vs);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
    }
}