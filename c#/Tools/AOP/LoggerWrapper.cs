using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using NLog;

namespace BouilholLib.Helper
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

        private string GetMessageLeave(object result)
        {
            return string.Format("OnExit : {0}", result.ToString());
        }

        private string GetMessageEnter(string name, object[] args)
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
