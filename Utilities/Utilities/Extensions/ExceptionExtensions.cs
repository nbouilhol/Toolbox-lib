using System;

namespace Utilities.Extensions
{
    public static class ExceptionExtensions
    {
        public static Exception GetOriginalException(this Exception ex)
        {
            if (ex.InnerException == null) return ex;

            return ex.InnerException.GetOriginalException();
        }

        public static string GetLogMessage(this Exception ex)
        {
            if (ex.InnerException == null) return "";
            return string.Format("{0} > {1} ", ex.InnerException.Message, GetLogMessage(ex.InnerException));
        }
    }
}