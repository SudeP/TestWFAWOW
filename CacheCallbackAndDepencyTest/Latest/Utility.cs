using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;

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
            try
            {
                using FileStream fileStream = new FileStream(
                    filePath,
                    FileMode.OpenOrCreate,
                    FileAccess.Read);

                BinaryFormatter binaryFormatter = new BinaryFormatter();

                return (T)binaryFormatter.Deserialize(fileStream);
            }
            catch (Exception exception)
            {
                Trace.Fail(exception.ToString());
                return default;
            }
        }
        internal static string PathMap(params string[] vs) => Statics.Server.MapPath(Path.Combine(vs));
        internal static string PathCombine(params string[] vs) => Path.Combine(vs);
        internal static string GuidFileName(Guid guid) => guid.ToString() + Statics.defaultFileExtesion;
    }
    internal class ProviderUtility
    {
        internal static string Impure2Pure(string impureKey) => IOUtility.PathCombine(impureKey.Split(new string[] { Statics.Request.Path }, StringSplitOptions.None).First(), Statics.Request.Path);
        internal static bool IsFirstPick(string key) => key.Split(new string[] { Statics.Request.Path }, StringSplitOptions.None).Last().Length == 0;
    }
}