using System.IO;
using System.Reflection;

namespace Utilities.Helpers
{
    public static class PathHelper
    {
        public static string GetLocalPath<T>(string filePath)
        {
            Assembly assembly = Assembly.GetAssembly(typeof(T));
            string path = assembly != null ? Path.GetDirectoryName(assembly.CodeBase.Replace(@"file:///", @"").Replace(@"file://", @"//")) : "";

            if (!path.EndsWith("\\")) path += "\\";
            string result = path + Path.GetFileName(filePath);
            return !string.IsNullOrEmpty(result) ? result : filePath;
        }
    }
}