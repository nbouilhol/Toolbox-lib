using System;
using System.Globalization;
using Utilities.Helpers;

namespace Utilities.Extensions
{
    public static class StringExtension
    {
        public static decimal DistanceToInPercent(this string source, string target)
        {
            return 1 - ((decimal) Levenshtein.CalculateDistance(source, target)/target.Length);
        }

        public static int DistanceTo(this string source, string target)
        {
            return Levenshtein.CalculateDistance(source, target);
        }

        public static int ToInt(this string source)
        {
            int result;
            return source != null && int.TryParse(source, out result)
                ? result
                : 0;
        }

        public static bool ToBool(this string source)
        {
            bool result;
            return source != null && bool.TryParse(source, out result) && result;
        }

        public static bool ToBool(this string source, bool defaultValue)
        {
            bool result;
            return source != null && bool.TryParse(source, out result) ? result : defaultValue;
        }

        public static short ToShort(this int source)
        {
            try
            {
                return Convert.ToInt16(source);
            }
            catch (Exception)
            {
                return default(short);
            }
        }

        public static int ToInt(this double source)
        {
            try
            {
                return Convert.ToInt32(source);
            }
            catch (Exception)
            {
                return default(int);
            }
        }

        public static double ToDouble(this string source)
        {
            double result;
            return double.TryParse(source, out result)
                ? result
                : default(double);
        }

        public static TimeSpan ToTimeSpan(this string value, string format)
        {
            TimeSpan result;
            return value != null && TimeSpan.TryParseExact(value, format, CultureInfo.InvariantCulture, out result)
                ? result
                : TimeSpan.Zero;
        }
    }
}