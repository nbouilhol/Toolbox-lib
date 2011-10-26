using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BouilholLib.Helper
{
    public static class IntExtension
    {
        public static bool ToBool(this int num)
        {
            return num == 0 ? false : true;
        }

        public static double ToDouble(this decimal num)
        {
            return Convert.ToDouble(num);
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
    }
}
