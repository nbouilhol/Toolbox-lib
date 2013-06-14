using System.Collections.Concurrent;

namespace Utilities.NLog
{
    public static class LogExtensions
    {
        private static readonly ConcurrentDictionary<string, ILog> _dictionary = new ConcurrentDictionary<string, ILog>();

        public static ILog Log<T>(this T type)
        {
            string objectName = typeof(T).FullName;
            return Log(objectName);
        }

        public static ILog Log(this string objectName)
        {
            return _dictionary.GetOrAdd(objectName, NLog.Log.GetLoggerFor);
        }
    }
}