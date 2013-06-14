using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using NLog;
using Utilities.Extensions;

namespace Utilities.NLog
{
    [DebuggerStepThrough]
    public static class Guard
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static void RequireNotNull<T>(Expression<Func<T>> reference, T value)
        {
            Contract.Requires(value != null);

            if (value == null)
                throw new ArgumentNullException(GetParameterName(reference), "Parameter cannot be null.");
        }

        public static void RequireNotNullOrEmpty(Expression<Func<string>> reference, string value)
        {
            Contract.Requires(value.Length != 0);

            RequireNotNull<string>(reference, value);
            if (value.Length == 0)
                throw new ArgumentException(GetParameterName(reference), "Parameter cannot be empty.");
        }

        public static void Against<TException>(bool assertion, string message) where TException : Exception
        {
            Contract.Requires(assertion);

            if (assertion)
            {
                TException exception = TypeExtensions.GetInstance<string, TException>(message);
                logger.ErrorException(message, exception);
                throw exception;
            }
        }

        [DebuggerStepThrough]
        public static void LogAndThrow<TException>(TException exception) where TException : Exception
        {
            logger.ErrorException("Exception occured", exception);
            throw exception;
        }

        public static void Requires(bool assertion, string message)
        {
            Contract.Requires(assertion);

            if (assertion)
            {
                ArgumentException exception = new ArgumentException(message);
                logger.ErrorException(string.Format("Requires : {0} : Failed", message), exception);
                throw exception;
            }
        }

        public static void RequireNotNull(object value, string message)
        {
            Contract.Requires(value != null);

            if (value == null)
            {
                ArgumentNullException exception = new ArgumentNullException(message);
                logger.ErrorException(string.Format("Require Not Null : {0} : Failed", message), exception);
                throw exception;
            }
        }

        public static void Ensures<TException>(bool assertion, string message) where TException : Exception
        {
            Contract.Requires(assertion);

            if (assertion)
            {
                TException exception = TypeExtensions.GetInstance<string, TException>(message);
                logger.ErrorException(string.Format("Ensures : {0} : Failed", message), exception);
                throw exception;
            }
        }

        //private static Func<string, TException> CreateExceptionExpression<TException>()
        //{
        //    ParameterExpression messageParameter = Expression.Parameter(typeof(string), "message");
        //    ConstructorInfo constructorInfo = typeof(TException).GetConstructor(new[] { typeof(string) });
        //    return Expression.Lambda<Func<string, TException>>(Expression.New(constructorInfo, messageParameter), messageParameter).Compile();
        //}

        private static string GetParameterName(Expression reference)
        {
            LambdaExpression lambda = reference as LambdaExpression;
            MemberExpression member = lambda.Body as MemberExpression;

            return member.Member.Name;
        }
    }
}