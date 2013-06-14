using System;
using NLog;

namespace Utilities.NLog
{
    public class NLogLog : ILog, ILog<NLogLog>
    {
        private Logger _logger;

        public void Debug(string message, params object[] formatting)
        {
            if (_logger.IsDebugEnabled) _logger.Debug(message, formatting);
        }

        public void Debug(Func<string> message)
        {
            if (_logger.IsDebugEnabled) _logger.Debug(message());
        }

        public void Error(string message, params object[] formatting)
        {
            _logger.Error(message, formatting);
        }

        public void Error(Func<string> message)
        {
            _logger.Error(message());
        }

        public void Fatal(string message, params object[] formatting)
        {
            _logger.Fatal(message, formatting);
        }

        public void Fatal(Func<string> message)
        {
            _logger.Fatal(message());
        }

        public void Info(string message, params object[] formatting)
        {
            if (_logger.IsInfoEnabled) _logger.Info(message, formatting);
        }

        public void Info(Func<string> message)
        {
            if (_logger.IsInfoEnabled) _logger.Info(message());
        }

        public void InitializeFor(string loggerName)
        {
            _logger = LogManager.GetLogger(loggerName);
        }

        public void Warn(string message, params object[] formatting)
        {
            if (_logger.IsWarnEnabled) _logger.Warn(message, formatting);
        }

        public void Warn(Func<string> message)
        {
            if (_logger.IsWarnEnabled) _logger.Warn(message());
        }
    }
}