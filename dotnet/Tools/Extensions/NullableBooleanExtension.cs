namespace Mvc.Utilitaires.Extensions
{
    #region
    using System;

    #endregion

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
            int res;
            try
            {
                res = value.HasValue
                          ? Convert.ToInt32(value)
                          : defaultValue;
            } catch (Exception)
            {
                res = 0;
            }
            return res;
        }

        /// <summary>
        /// Convert to Integer.
        /// </summary>
        ///<param name="value"></param>
        /// <returns></returns>
        public static int ToInt(this bool? value)
        {
            int res;
            try
            {
                res = value.HasValue
                          ? Convert.ToInt32(value)
                          : 0;
            } catch (Exception)
            {
                res = 0;
            }
            return res;
        }
    }
}