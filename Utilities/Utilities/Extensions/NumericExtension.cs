using System;
using System.Collections.Generic;

namespace Utilities.Extensions
{
    public static class NumericExtension
    {
        public static bool ToBool(this int num)
        {
            return num == 0 ? false : true;
        }

        public static int ToInt(this bool num)
        {
            return num ? 0 : 1;
        }

        public static double ToDouble(this decimal num)
        {
            return Convert.ToDouble(num);
        }

        public static bool IsValidMonth(this int num)
        {
            return num > 0 && num <= 12;
        }

        public static double ToDouble(this decimal? num)
        {
            if (num == null)
                return 0;
            return Convert.ToDouble(num);
        }

        public static decimal ToDecimal(this double num)
        {
            return Convert.ToDecimal(num);
        }

        public static decimal ToPercent(this decimal value, decimal total)
        {
            return ((total - value) / total) * 100;
        }

        public static IEnumerable<int> ToInts(this bool num)
        {
            return num == false ? new List<int> { 0 } : new List<int> { 0, 1 };
        }
    }
}
