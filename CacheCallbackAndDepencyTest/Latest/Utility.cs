using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Caching;
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
        internal static string PathMap(params string[] vs) => Statics.Server.MapPath(Path.Combine(vs));
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
        internal static string Impure2Pure(string impureKey) => string.Concat(impureKey.Split(new string[] { ToLower(Statics.Request.Path) }, StringSplitOptions.None).First(), ToLower(Statics.Request.Path));
        internal static bool IsFirstPick(string key) => key.Split(new string[] { ToLower(Statics.Request.Path) }, StringSplitOptions.None).Last().Length == 0;
        internal static string ToLower(string text) => CultureInfo.InvariantCulture.TextInfo.ToLower(text);
        internal static void UseSnapShot(object oce)
        {
            var toc = typeof(OutputCache);
            object crr = toc.GetMethods(Statics.bf)
                .First(m => m.Name == "Convert" && m.ReturnParameter.ParameterType.Name == "CachedRawResponse")
                .Invoke(null, new object[] { oce });

            object raw = crr.GetType().GetField("_rawResponse", Statics.bf).GetValue(crr);

            Type tr = Statics.Response.GetType();
            tr.GetMethod("UseSnapShot", Statics.bf).Invoke(Statics.Response, new object[] { raw, true });
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