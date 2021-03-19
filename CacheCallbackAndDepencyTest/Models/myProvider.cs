using System;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace WebApplication1.Helpers
{
    [Serializable]
    internal class CacheItem
    {
        public DateTime Expires;
        public object Item;
    }

    public class FileCacheProvider : OutputCacheProvider
    {
        private string _cachePath;


        private string CachePath
        {
            get
            {
                if (!string.IsNullOrEmpty(_cachePath))
                    return _cachePath;

                _cachePath = ConfigurationManager.AppSettings["OutputCachePath"];

                var context = System.Web.HttpContext.Current;

                if (context != null)
                {
                    _cachePath = context.Server.MapPath(_cachePath);
                    if (!_cachePath.EndsWith("\\"))
                        _cachePath += "\\";
                }

                return _cachePath;
            }
        }

        public override object Add(string key, object entry, DateTime utcExpiry)
        {
            string strCacheKey = HttpContext.Current.Request.Url.PathAndQuery.ToString();
            // parametreler falan okunuyor buradan

            // neden her seferinde yeni cache alıyor onu anlamadım....

            Debug.WriteLine("URL --> " + strCacheKey);
            Debug.WriteLine("Cache.Add(" + key + ", " + entry + ", " + utcExpiry + ")");

            var path = GetPathFromKey(key);



            if (System.IO.File.Exists(path))
                return entry;

            using (var file = System.IO.File.OpenWrite(path))
            {
                var item = new CacheItem { Expires = utcExpiry, Item = entry };
                var formatter = new BinaryFormatter();
                formatter.Serialize(file, item);
            }

            return entry;
        }

        public override object Get(string key)
        {
            Debug.WriteLine("Cache.Get(" + key + ")");

            var path = GetPathFromKey(key);

            if (!System.IO.File.Exists(path))
                return null;

            CacheItem item = null;

            using (var file = System.IO.File.OpenRead(path))
            {
                var formatter = new BinaryFormatter();
                item = (CacheItem)formatter.Deserialize(file);
            }

            if (item == null || item.Expires <= DateTime.Now.ToUniversalTime())
            {
                Remove(key);
                return null;
            }

            return item.Item;
        }

        public override void Remove(string key)
        {
            Debug.WriteLine("Cache.Remove(" + key + ")");

            var path = GetPathFromKey(key);

            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
        }

        public override void Set(string key, object entry, DateTime utcExpiry)
        {
            Debug.WriteLine("Cache.Set(" + key + ", " + entry + ", " + utcExpiry + ")");

            var item = new CacheItem { Expires = utcExpiry, Item = entry };
            var path = GetPathFromKey(key);

            using (var file = System.IO.File.OpenWrite(path))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(file, item);
            }
        }

        private string GetPathFromKey(string key)
        {
            return CachePath + MD5(key) + ".txt";
        }

        private string MD5(string s)
        {
            var provider = new MD5CryptoServiceProvider();
            var bytes = Encoding.UTF8.GetBytes(s);
            var builder = new StringBuilder();

            bytes = provider.ComputeHash(bytes);

            foreach (var b in bytes)
                builder.Append(b.ToString("x2").ToLower());

            return builder.ToString();
        }
    }
}