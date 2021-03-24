using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Routing;

namespace HybridServer
{
    internal class IOUtility
    {
        internal static bool Serialize(string filePath, object serializeObject)
        {
            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(
                    filePath,
                    FileMode.OpenOrCreate,
                    FileAccess.Write);

                BinaryFormatter binaryFormatter = new BinaryFormatter();

                binaryFormatter.Serialize(fileStream, serializeObject);

                return true;
            }
            catch (Exception exception)
            {
#if DEBUG
                Trace.Fail(exception.ToString());
#endif
                return false;
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Dispose();
            }
        }
        internal static T Deserialize<T>(string filePath)
        {
            if (!File.Exists(filePath))
                return default;

            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(
                    filePath,
                    FileMode.Open,
                    FileAccess.Read);

                if (!fileStream.CanRead)
                    return default;

                BinaryFormatter binaryFormatter = new BinaryFormatter();

                object t = binaryFormatter.Deserialize(fileStream);

                return (T)t;
            }
            catch (Exception exception)
            {
#if DEBUG
                Trace.Fail(exception.ToString());
#endif
                return default;
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Dispose();
            }
        }
        internal static string PathMap(HttpContext httpContext, params string[] vs) => httpContext.Server.MapPath(Path.Combine(vs));
        internal static string PathCombine(params string[] vs) => Path.Combine(vs);
        internal static string CreateFileName() => Guid.NewGuid().ToString() + Statics.defaultFileExtesion;
        internal static string KeyCrypte(string @string)
        {
            var provider = new MD5CryptoServiceProvider();
            var bytes = Encoding.UTF8.GetBytes(@string);
            var builder = new StringBuilder();

            bytes = provider.ComputeHash(bytes);

            foreach (var b in bytes)
                builder.Append(b.ToString("x2").ToLower());

            return builder.ToString();
        }
    }
    internal class ProviderUtility
    {
        internal static string Impure2Pure(HttpContext httpContext, string impureKey) => string.Concat(impureKey.Split(new string[] { ToLower(httpContext.Request.Path) }, StringSplitOptions.None).First(), ToLower(httpContext.Request.Path));
        internal static bool IsFirstPick(HttpContext httpContext, string key) => key.Split(new string[] { ToLower(httpContext.Request.Path) }, StringSplitOptions.None).Last().Length == 0;
        internal static string ToLower(string text) => CultureInfo.InvariantCulture.TextInfo.ToLower(text);
        internal static void CollectorRun(int milliSecond)
        {
            if (Statics.Collector == null)
                Statics.Collector = Task.Factory.StartNew(() =>
                {
                    do
                    {
                        KeyValuePair<string, HSSettings>[] vs = Statics.HSSettings.ToArray();

                        for (int i = 0; i < vs.Length; i++)
                        {
                            HSSettings settings = vs[i].Value;
                            if (settings.isChange)
                            {
                                settings.isChange = false;

                                IOUtility.Serialize(settings.PhysicalPath, settings);

                                string[] subFolders = Directory.GetDirectories(settings.RootPath);

                                foreach (string subFolder in subFolders)
                                {
                                    string[] files = Directory.GetFiles(subFolder);

                                    DateTime newestDate = DateTime.MinValue;

                                    string newestFile = string.Empty;

                                    foreach (string file in files)
                                    {
                                        FileInfo fileInfo = new FileInfo(file);
                                        if (fileInfo.LastWriteTime > newestDate)
                                        {
                                            newestDate = fileInfo.LastWriteTime;
                                            newestFile = file;
                                        }
                                    }

                                    files = files.Where(f => f != newestFile).ToArray();

                                    foreach (string file in files)
                                        File.Delete(file);
                                }
                            }
                        }
                        Thread.Sleep(milliSecond);
                    } while (true);
                });
        }
        internal static Task<HttpContext> RequestClone(HttpContext httpContext)
        {
            return Task.Factory.StartNew((obj) =>
            {
                Uri uri = (Uri)obj;

                HttpContext httpContext = new HttpContext(new HttpRequest(
                    string.Empty,
                    uri.AbsoluteUri,
                    uri.Query), new HttpResponse(new StringWriter()));

                HttpContext.Current = httpContext;

                RouteData routeData = RouteUtils.GetRouteDataByUrl(
                    httpContext
                    .Request
                    .AppRelativeCurrentExecutionFilePath);

                Controller controller =
                (Controller)(DependencyResolver.Current.GetService<IControllerFactory>() ?? new DefaultControllerFactory())
                 .CreateController(
                    httpContext.Request.RequestContext,
                    routeData.Values["controller"].ToString());

                controller.ControllerContext = new ControllerContext(
                    new HttpContextWrapper(httpContext),
                    routeData,
                    controller);

                httpContext.Response.GetType()
                    .GetMethods(Statics.bf)
                    .FirstOrDefault(m => m.Name == "InitResponseWriter")
                    .Invoke(httpContext.Response, null);

                controller
                .ActionInvoker
                .InvokeAction(
                    controller.ControllerContext,
                    routeData.Values["action"].ToString());

                return httpContext;
            }, new Uri(httpContext.Request.Url.OriginalString));
        }
        internal static object GetSnapShot(HttpContext context)
        {
            var toc = typeof(OutputCache);

            object httpRawResponse = context.Response.GetType()
                .GetMethods(Statics.bf)
                .Where(pi => pi.Name == "GetSnapshot")
                .ToList()
                .FirstOrDefault()
                .Invoke(context.Response, null);

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
                if (elem != null)
                {
                    var et = elem.GetType();
                    if (et != null)
                    {
                        var etn = et.Name;
                        var em = et.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToList();
                        var ef = et.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToList();
                        var ep = et.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToList();
                        if (etn == "HttpFileResponseElement")
                        {
                            var fileName = ep.FirstOrDefault(p => p.Name == "FileName");
                            var fileNameVal = (string)fileName.GetValue(elem);
                            var offset = ep.FirstOrDefault(p => p.Name == "Offset");
                            var offsetVal = (long)offset.GetValue(elem);
                            var getSize = em.FirstOrDefault(m => m.Name == "GetSize");

                            responseElements.Add(new FileResponseElement(fileNameVal, offsetVal, (long)getSize.Invoke(elem, null)));
                        }
                        else if (etn == "HttpSubstBlockResponseElement")
                        {

                            var callback = ep.FirstOrDefault(p => p.Name == "Callback");
                            var callbackVal = (HttpResponseSubstitutionCallback)callback.GetValue(elem);

                            responseElements.Add(new SubstitutionResponseElement(callbackVal));
                        }
                        else
                        {
                            var getBytes = em.FirstOrDefault(m => m.Name == "System.Web.IHttpResponseElement.GetBytes");

                            byte[] b = (byte[])getBytes.Invoke(elem, null);
                            long length = (b != null) ? b.Length : 0;
                            responseElements.Add(new MemoryResponseElement(b, length));
                        }
                    }
                }
            }
            return responseElements;
        }
    }
    public static class RouteUtils
    {
        public static RouteData GetRouteDataByUrl(string url)
        {
            return RouteTable.Routes.GetRouteData(new RewritedHttpContextBase(url));
        }

        private class RewritedHttpContextBase : HttpContextBase
        {
            private readonly HttpRequestBase mockHttpRequestBase;

            public RewritedHttpContextBase(string appRelativeUrl)
            {
                this.mockHttpRequestBase = new MockHttpRequestBase(appRelativeUrl);
            }


            public override HttpRequestBase Request
            {
                get
                {
                    return mockHttpRequestBase;
                }
            }

            private class MockHttpRequestBase : HttpRequestBase
            {
                private readonly string appRelativeUrl;

                public MockHttpRequestBase(string appRelativeUrl)
                {
                    this.appRelativeUrl = appRelativeUrl;
                }

                public override string AppRelativeCurrentExecutionFilePath
                {
                    get { return appRelativeUrl; }
                }

                public override string PathInfo
                {
                    get { return ""; }
                }
            }
        }
    }
}