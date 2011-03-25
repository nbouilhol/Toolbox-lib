using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Mvc.Utilitaires.Extensions
{
    public static class StringExtension
    {
        public static string UppercaseFirst(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return null;
            return char.ToUpper(s[0], CultureInfo.CurrentCulture) + s.Substring(1);
        }

        public static DateTime? FromQueryStringToDateTime(this string date)
        {
            DateTime tmp;

            if (!DateTime.TryParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out tmp))
                return null;

            return tmp;
        }

        public static int? ToNullableInt32(this string s)
        {
            int i;
            if (string.IsNullOrEmpty(s)) return null;
            if (Int32.TryParse(s, out i)) return i;
            return null;
        }

        public static DateTime? ToDateTime(this string date)
        {
            DateTime tmp;
            if (!DateTime.TryParse(date, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces, out tmp))
                return null;
            if (tmp < DateTime.MinValue || tmp < SqlDateTime.MinValue.Value)
                return null;
            return tmp;
        }

        public static string Summarize(this string text, int length, string appendToEnd)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            if (text.Length > length)
                return string.Concat(text.Substring(0, length - 3), appendToEnd);
            return text;
        }

        private static IEnumerable<char> RemoveDiacritics(string src, bool compatNorm, Func<char, char> customFolding)
        {
            foreach (char c in src.Normalize(compatNorm ? NormalizationForm.FormKD : NormalizationForm.FormD))
                switch (CharUnicodeInfo.GetUnicodeCategory(c))
                {
                    case UnicodeCategory.NonSpacingMark:
                    case UnicodeCategory.SpacingCombiningMark:
                    case UnicodeCategory.EnclosingMark:
                        break;
                    default:
                        yield return customFolding(c);
                        break;
                }
        }

        public static string RemoveAccent(this string src, bool compatNorm, Func<char, char> customFolding)
        {
            if (string.IsNullOrEmpty(src))
                return src;
            StringBuilder sb = new StringBuilder();
            foreach (char c in RemoveDiacritics(src, compatNorm, customFolding))
                sb.Append(c);
            return sb.ToString();
        }

        public static string RemoveAccent(this string src, bool compatNorm)
        {
            if (string.IsNullOrEmpty(src))
                return src;
            return src.RemoveAccent(compatNorm, c => c);
        }

        /// <summary>
        /// Convert to Integer specifying a default value.
        /// </summary>
        ///<param name="str"></param>
        ///<param name="defaultValue"></param>
        /// <returns></returns>
        public static int ToInt(this string str, int defaultValue)
        {
            if (!string.IsNullOrEmpty(str))
            {
                int res;

                return int.TryParse(str, out res)
                           ? res
                           : defaultValue;
            }
            return 0;
        }

        /// <summary>
        /// Convert to Integer.
        /// </summary>
        ///<param name="str"></param>
        /// <returns></returns>
        /// <exception cref="OverflowException"><c>OverflowException</c>.</exception>
        public static int ToInt(this string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                int res;

                return int.TryParse(str, out res)
                           ? res
                           : default(int);
            }
            return 0;
        }

        /// <summary>
        /// Convert to Long specifying a default value.
        /// </summary>
        ///<param name="str"></param>
        ///<param name="defaultValue"></param>
        /// <returns></returns>
        /// <exception cref="OverflowException"><c>OverflowException</c>.</exception>
        public static long ToLong(this string str, long defaultValue)
        {
            long res;
            try
            {
                res = !string.IsNullOrEmpty(str)
                          ? long.Parse(str)
                          : 0;
            }
            catch (OverflowException)
            {
                throw;
            }
            catch (Exception)
            {
                res = defaultValue;
            }
            return res;
        }

        /// <summary>
        /// Convert to Long.
        /// </summary>
        ///<param name="str"></param>
        /// <returns></returns>
        public static long ToLong(this string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                long res;

                return long.TryParse(str, out res)
                           ? res
                           : 0;
            }
            return 0;
        }

        /// <summary>
        /// Convert to Decimal specifying a default value.
        /// </summary>
        ///<param name="str"></param>
        ///<param name="defaultValue"></param>
        /// <returns></returns>
        public static decimal ToDec(this string str, decimal defaultValue)
        {
            if (!string.IsNullOrEmpty(str))
            {
                decimal res;
                return decimal.TryParse(str, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out res)
                           ? res
                           : defaultValue;
            }
            return 0;
        }

        /// <summary>
        /// Convert to Decimal.
        /// </summary>
        ///<param name="str"></param>
        /// <returns></returns>
        public static decimal ToDec(this string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                decimal res;
                return decimal.TryParse(str, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out res)
                           ? res
                           : 0;
            }
            return 0;
        }

        /// <summary>
        /// Convert to Boolean specifying a default value.
        /// </summary>
        ///<param name="str"></param>
        ///<param name="defaultValue"></param>
        /// <returns></returns>
        public static bool ToBool(this string str, bool defaultValue)
        {
            if (!string.IsNullOrEmpty(str))
            {
                bool res;

                return bool.TryParse(str, out res)
                           ? res
                           : defaultValue;
            }
            return false;
        }

        /// <summary>
        /// Convert to Boolean.
        /// </summary>
        ///<param name="str"></param>
        /// <returns></returns>
        public static bool ToBool(this string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                bool res;

                return bool.TryParse(str, out res)
                           ? res
                           : false;
            }
            return false;
        }

        /// <summary>
        /// Convert to DateTime specifying a default value.
        /// </summary>
        ///<param name="str"></param>
        ///<param name="defaultValue"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string str, DateTime defaultValue)
        {
            if (!string.IsNullOrEmpty(str))
            {
                DateTime res;

                return DateTime.TryParse(str, out res)
                           ? res
                           : defaultValue;
            }
            return DateTime.MinValue;
        }

        /// <summary>
        /// convertit une chaine de caractère en date en specifiant exactement le bon format 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="format">ddMMMyyyy ou yyyyMMdd ...</param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string str, string format)
        {
            if (!string.IsNullOrEmpty(str))
            {
                DateTime res;

                return DateTime.TryParseExact(str, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out res)
                           ? res
                           : DateTime.MinValue;
            }
            return DateTime.MinValue;
        }

        /// <summary>
        /// Convert to DateTime.
        /// </summary>
        ///<param name="str"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                DateTime res;

                return DateTime.TryParse(str, out res)
                           ? res
                           : DateTime.MinValue;
            }

            return DateTime.MinValue;
        }

        /// <summary>
        /// Indicates whether the regular expression finds a match in the input string using the regular expression specified in the pattern parameter.
        /// </summary>
        ///<param name="value">String to match</param>
        ///<param name="regularExpression">Regular expression, eg. [a-Z]{3}</param>
        ///<param name="matchEntirely">Return true only if string was matched entirely</param>
        /// <returns></returns>
        public static bool IsMatch(this string value, string regularExpression, bool matchEntirely)
        {
            return Regex.IsMatch(value, matchEntirely
                                            ? "\\A" + regularExpression + "\\z"
                                            : regularExpression);
        }

    }
}
