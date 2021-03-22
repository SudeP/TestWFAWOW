using System;
using System.Web;

namespace HybridServer
{
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
        internal Guid Guid { get; }
    }
}