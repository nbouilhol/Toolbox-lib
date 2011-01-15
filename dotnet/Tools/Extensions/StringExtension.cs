using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace SuiviCRA.Utilitaires.Extensions
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
    }
}
