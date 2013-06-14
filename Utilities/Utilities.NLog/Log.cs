using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using Utilities.SqlHelpers.Mapper;

namespace Utilities.NLog
{
    public static class Log
    {
        private static Type logType = typeof(NullLog);
        private static Func<ILog> builder;
        private static ILog testLogger;

        public static ILog GetLoggerFor(string objectName)
        {
            if (testLogger != null) return testLogger;

            ILog logger = builder();
            if (logger != null) logger.InitializeFor(objectName);

            return logger;
        }

        public static void InitializeWith<T>() where T : ILog, new()
        {
            logType = typeof(T);
            builder = CreateActivatorDelegate<T>() as Func<ILog>;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void InitializeWith(ILog testLoggerType)
        {
            logType = testLoggerType.GetType();
            testLogger = testLoggerType;
        }

        private static Func<T> CreateActivatorDelegate<T>() where T : ILog, new()
        {
            ConstructorInfo constructor = typeof(T).GetConstructor(Type.EmptyTypes);
            if (constructor == null) return () => { throw MappingException.NoParameterlessConstructor(typeof(T)); };
            return Expression.Lambda<Func<T>>(Expression.New(constructor)).Compile();
        }
    }
}