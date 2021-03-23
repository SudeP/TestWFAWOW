using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace HybridServer
{
    internal class IOUtility
    {
        internal static bool Serialize(string filePath, object serializeObject)
        {
            try
            {
                using FileStream fileStream = new FileStream(
                    filePath,
                    FileMode.OpenOrCreate,
                    FileAccess.Write);

                BinaryFormatter binaryFormatter = new BinaryFormatter();

                binaryFormatter.Serialize(fileStream, serializeObject);

                return true;
            }
            catch (Exception exception)
            {
                Trace.Fail(exception.ToString());
                return false;
            }
        }
        internal static T Deserialize<T>(string filePath)
        {
            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(
                    filePath,
                    FileMode.OpenOrCreate,
                    FileAccess.Read);

                if (fileStream.Length == 0)
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
        internal static string GuidFileName(Guid guid) => guid.ToString() + Statics.defaultFileExtesion;
    }
    internal class ProviderUtility
    {
        internal static string Impure2Pure(string impureKey) => IOUtility.PathCombine(impureKey.Split(new string[] { ToLower(Statics.Request.Path) }, StringSplitOptions.None).First(), Statics.Request.Path);
        internal static bool IsFirstPick(string key) => key.Split(new string[] { ToLower(Statics.Request.Path) }, StringSplitOptions.None).Last().Length == 0;
        internal static string ToLower(string text) => CultureInfo.InvariantCulture.TextInfo.ToLower(text);
    }
}