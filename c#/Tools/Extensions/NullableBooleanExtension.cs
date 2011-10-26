namespace BouilholLib.Helper
{
    using System;

    public static class NullableBooleanExtension
    {
        /// <summary>
        /// Convert to Integer specifying a default value.
        /// </summary>
        ///<param name="value"></param>
        ///<param name="defaultValue"></param>
        /// <returns></returns>
        public static int ToInt(this bool? value, int defaultValue)
        {
            return value.HasValue ? Convert.ToInt32(value) : defaultValue;
        }

        /// <summary>
        /// Convert to Integer.
        /// </summary>
        ///<param name="value"></param>
        /// <returns></returns>
        public static int ToInt(this bool? value)
        {
            return value.HasValue ? Convert.ToInt32(value) : 0;
        }
		
		public static IEnumerable<int> ToInts(this bool num)
        {
            return num == false ? new List<int> { 0 } : new List<int> { 0, 1 };
        }
		
		public static int ToInt(this bool num)
        {
            return num == false ? 0 : 1;
        }
    }
}