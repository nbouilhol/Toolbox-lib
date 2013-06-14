using System;

namespace Utilities.NLog
{
    public interface ILog
    {
        void Debug(string message, params object[] formatting);

        void Debug(Func<string> message);

        void Error(string message, params object[] formatting);

        void Error(Func<string> message);

        void Fatal(string message, params object[] formatting);

        void Fatal(Func<string> message);

        void Info(string message, params object[] formatting);

        void Info(Func<string> message);

        void InitializeFor(string loggerName);

        void Warn(string message, params object[] formatting);

        void Warn(Func<string> message);
    }

    public interface ILog<T>
    {
    }
}