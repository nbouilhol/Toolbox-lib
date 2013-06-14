using System;
using System.Diagnostics.Contracts;

namespace Utilities.SqlHelpers.Mapper
{
    [Serializable]
    public class MappingException : Exception
    {
        public MappingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public MappingException(string message)
            : base(message)
        {
        }

        public static MappingException InvalidCast(string column, Exception innerException)
        {
            string message = string.Format("Could not map the property '{0}' as its data type does not match the database.", column);
            return new MappingException(message, innerException);
        }

        public static MappingException NoParameterlessConstructor(Type type)
        {
            Contract.Requires(type != null);

            string message = "Could not find a parameterless constructor on the type '{0}'.";
            message = string.Format(message, type.FullName);
            return new MappingException(message);
        }
    }
}