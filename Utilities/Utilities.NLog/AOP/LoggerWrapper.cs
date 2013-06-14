using System;
using System.Linq;
using NLog;
using Utilities.AOP;

namespace Utilities.NLog.AOP
{
    public class LoggerWrapper<T> : BaseWrapper<T> where T : class
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public LoggerWrapper(T source)
            : base(source)
        {
        }

        public override void OnEntry(T source, string methodName, object[] args)
        {
            string messageEnter = GetMessageEnter(methodName, args);

            Console.Out.WriteLine(messageEnter);
            logger.Info(messageEnter);
        }

        public override void OnExit(T source, string methodName, object[] args, object result)
        {
            string messageLeave = GetMessageLeave(result);

            Console.Out.WriteLine(messageLeave);
            logger.Info(messageLeave);
        }

        private static string GetMessageLeave(object result)
        {
            return string.Format("OnExit : {0}", result.ToString());
        }

        private static string GetMessageEnter(string name, object[] args)
        {
            return string.Format("OnEntry : {0} With : {1}", name, string.Join(", ", args != null ? args.Select(arg => arg.ToString()) : null));
        }
    }

    public static class LoggerWrapperExtension
    {
        public static dynamic ToLogger<T>(this T source) where T : class
        {
            return new LoggerWrapper<T>(source);
        }
    }
}