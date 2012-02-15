using System;
using System.Linq;
using Utilities.Helpers;

namespace Utilities.Extensions
{
    public static class EnumExtension
    {
        public static string GetStringValue(this Enum value)
        {
            StringValueAttribute attribute = value.GetType().GetField(value.ToString()).GetCustomAttributes(false).OfType<StringValueAttribute>().FirstOrDefault();
            return attribute != null ? attribute.StringValue : null;
        }
    }
}
