using Moq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Mvc.Async;
using System.Web.Routing;

namespace HybridServer
{
    internal sealed class OutputCacheHybridServerProvider : OutputCacheProvider
    {
        public OutputCacheHybridServerProvider()
        {
            if (Statics.Collector == null)
                Statics.Collector = Task.Factory.StartNew(() =>
                {
                    do
                    {
                        KeyValuePair<string, HSSettings>[] vs = Statics.HSSettings.ToArray();

                        for (int i = 0; i < vs.Length; i++)
                        {
                            if (vs[i].Value.isChange)
                            {
                                vs[i].Value.isChange = false;

                                IOUtility.Serialize(vs[i].Value.PhysicalPath, vs[i].Value);
                            }
                        }
                        Thread.Sleep(Statics.oneMinute);
                    } while (true);
                });
        }
        public override object Add(string key, object entry, DateTime utcExpiry)
        {
            HSSettings hSSettings = Statics.HSSettings.AddOrUpdate(
                key,
                _ => new HSSettings(key, entry),
                (_, oldHSSettings) => oldHSSettings.CachedVary != null ? oldHSSettings : oldHSSettings.Update(key, entry));

            return hSSettings.CachedVary;
        }
        public override object Get(string key)
        {
            if (ProviderUtility.IsFirstPick(key))
            {
                HSSettings hSSettings = Statics.HSSettings.GetOrAdd(
                    key,
                    k => new HSSettings(k, null));

                return hSSettings.CachedVary;
            }
            else
            {
                object outputCacheEntry = null;

                if (Statics.HSSettings.TryGetValue(ProviderUtility.Impure2Pure(key), out HSSettings hSSettings))
                    if (hSSettings.HSCache.TryGetValue(key, out HSCache hSCache))
                    {
                        outputCacheEntry = hSCache.OutputCacheEntry;

                        if (hSCache.UtcExpiry < DateTime.UtcNow && !hSCache.IsReloding)
                        {
                            string absoluteUri = Statics.Request.Url.AbsoluteUri;

                            Task.Factory.StartNew(() =>
                            {
                                HttpRequest httpRequest = new HttpRequest("", absoluteUri, "");
                                StringWriter stringWriter = new StringWriter();
                                HttpResponse httpResponse = new HttpResponse(stringWriter);
                                HttpContext httpContextMock = new HttpContext(httpRequest, httpResponse);

                                RouteData routeData = RouteUtils.GetRouteDataByUrl(httpContextMock.Request.AppRelativeCurrentExecutionFilePath);

                                var factory = DependencyResolver.Current.GetService<IControllerFactory>() ?? new DefaultControllerFactory();
                                Controller controller = (Controller)factory.CreateController(httpContextMock.Request.RequestContext, routeData.Values["controller"].ToString());

                                ControllerContext newContext = new ControllerContext(new HttpContextWrapper(httpContextMock), routeData, controller);

                                controller.ControllerContext = newContext;
                                HttpContext.Current = httpContextMock;

                                var tctr = httpContextMock.Response.GetType();

                                var v1 = tctr.GetMethods(Statics.bf).FirstOrDefault(m => m.Name == "InitResponseWriter");

                                var v2 = v1.Invoke(httpContextMock.Response, null);

                                bool isAction = (controller.ActionInvoker as AsyncControllerActionInvoker)
                                .InvokeAction(controller.ControllerContext, routeData.Values["action"].ToString());
                                Set(key, ProviderUtility.GetSnapShot(httpContextMock), DateTime.UtcNow.AddSeconds(10));
                            });
                        }
                    }

                return outputCacheEntry;
            }
        }
        public override void Remove(string key)
        {
            if (!ProviderUtility.IsFirstPick(key) && Statics.HSSettings.TryGetValue(ProviderUtility.Impure2Pure(key), out HSSettings hSSettings))
                hSSettings.TryRemove(key);
        }
        public override void Set(string key, object entry, DateTime utcExpiry)
        {
            if (Statics.HSSettings.TryGetValue(ProviderUtility.Impure2Pure(key), out HSSettings hSSettings))
            {
                hSSettings.QueueTasker.Add(() =>
                {
                    if (hSSettings.HSCache.TryGetValue(key, out HSCache hSCache) && hSCache.UtcExpiry > DateTime.UtcNow)
                        return;

                    hSCache = hSSettings.AddOrUpdate(
                        key,
                        _ => new HSCache(key, entry, utcExpiry, hSSettings),
                        (_, oldHSCache) => oldHSCache.UtcExpiry > DateTime.UtcNow ? oldHSCache : oldHSCache.Update(key, entry, utcExpiry, hSSettings));

                    IOUtility.Serialize(hSCache.PhysicalPath, hSCache.OutputCacheEntry);
                });
            }
        }
    }
}