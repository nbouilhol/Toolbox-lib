using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;

namespace BouilholLib.Helper
{
    public class Guard
    {
        private static NLog.Logger logger = LogManager.GetCurrentClassLogger();

        public static void Against<TException>(bool assertion, string message) where TException : Exception
        {
            if (assertion)
            {
                TException exception = (TException)Activator.CreateInstance(typeof(TException), message);
                logger.ErrorException(message, exception);
                throw exception;
            }
        }

        public static void LogAndThrow<TException>(TException exception) where TException : Exception
        {
            logger.ErrorException("Exception occured", exception);
            throw exception;
        }

        public static void Requires(bool assertion, string message)
        {
            if (assertion)
            {
                ArgumentException exception = new ArgumentException(message);
                logger.ErrorException(string.Format("Requires : {0} : Failed", message), exception);
                throw exception;
            }
        }

        public static void Ensures<TException>(bool assertion, string message) where TException : Exception
        {
            if (assertion)
            {
                TException exception = (TException)Activator.CreateInstance(typeof(TException), message);
                logger.ErrorException(string.Format("Ensures : {0} : Failed", message), exception);
                throw exception;
            }
        }
    }
}
