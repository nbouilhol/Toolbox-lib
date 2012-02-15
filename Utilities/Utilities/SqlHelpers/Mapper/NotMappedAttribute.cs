using System;

namespace Utilities.SqlHelpers.Mapper
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class NotMappedAttribute : Attribute
    {
    }
}
