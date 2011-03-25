using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;

namespace Mvc.Utilitaires.Extensions
{
    public static class DateTimeExtension
    {
        private const int DATEOFWEEK_MONDAY = 1;
        private const int DATEOFWEEK_TUESDAY = 2;
        private const int DATEOFWEEK_WEDNESDAY = 3;
        private const int DATEOFWEEK_THURSDAY = 4;
        private const int DATEOFWEEK_FRIDAY = 5;
        private const int DATEOFWEEK_SATURDAY = 6;
        private const int DATEOFWEEK_SUNDAY = 7;

        public static DateTime ThisWeekMonday(this DateTime date)
        {
            return date.AddDays(DATEOFWEEK_MONDAY - Convert.ToInt32(date.DayOfWeek, CultureInfo.InvariantCulture));
        }
        public static DateTime? ThisWeekMonday(this DateTime? date)
        {
            if (date == null)
                return date;
            return date.Value.AddDays(DATEOFWEEK_MONDAY - Convert.ToInt32(date.Value.DayOfWeek, CultureInfo.InvariantCulture));
        }
        public static DateTime ThisWeekTuesday(this DateTime date)
        {
            return date.AddDays(DATEOFWEEK_TUESDAY - Convert.ToInt32(date.DayOfWeek, CultureInfo.InvariantCulture));
        }
        public static DateTime ThisWeekWednesday(this DateTime date)
        {
            return date.AddDays(DATEOFWEEK_WEDNESDAY - Convert.ToInt32(date.DayOfWeek, CultureInfo.InvariantCulture));
        }
        public static DateTime ThisWeekThursday(this DateTime date)
        {
            return date.AddDays(DATEOFWEEK_THURSDAY - Convert.ToInt32(date.DayOfWeek, CultureInfo.InvariantCulture));
        }
        public static DateTime ThisWeekFriday(this DateTime date)
        {
            return date.AddDays(DATEOFWEEK_FRIDAY - Convert.ToInt32(date.DayOfWeek, CultureInfo.InvariantCulture));
        }
        public static DateTime ThisWeekSaturday(this DateTime date)
        {
            return date.AddDays(DATEOFWEEK_SATURDAY - Convert.ToInt32(date.DayOfWeek, CultureInfo.InvariantCulture));
        }
        public static DateTime ThisWeekSunday(this DateTime date)
        {
            return date.AddDays(DATEOFWEEK_SUNDAY - Convert.ToInt32(date.DayOfWeek, CultureInfo.InvariantCulture));
        }
        public static string ToQueryString(this DateTime? date)
        {
            if (date != null)
                return date.Value.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
            return null;
        }
        public static string ToQueryString(this DateTime date)
        {
            return date.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
        }
        public static DateTime NextDayInWeek(this DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Friday)
                return date.AddDays(3);
            if (date.DayOfWeek == DayOfWeek.Saturday)
                return date.AddDays(2);
            return date.AddDays(1);
        }
        public static DateTime PrevDayInWeek(this DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Monday)
                return date.AddDays(-3);
            if (date.DayOfWeek == DayOfWeek.Sunday)
                return date.AddDays(-2);
            return date.AddDays(-1);
        }
        public static DateTime NextMonday(this DateTime date)
        {
            return date.AddDays((7 + DATEOFWEEK_MONDAY) - Convert.ToInt32(date.DayOfWeek, CultureInfo.InvariantCulture));
        }
        public static DateTime PrevFriday(this DateTime date)
        {
            return date.AddDays(0 - ((7 - DATEOFWEEK_FRIDAY) + Convert.ToInt32(date.DayOfWeek, CultureInfo.InvariantCulture)));
        }
        public static IEnumerable<DateTime?> GetAllDaysBetweenDates(this DateTime? beginDate, DateTime? endDate)
        {
            if (beginDate == null)
                throw new ArgumentNullException("beginDate");
            if (endDate == null)
                throw new ArgumentNullException("endDate");

            int count = endDate.Value.Subtract(beginDate.Value).Days;
            for (int i = 0; i <= count; i++)
                yield return beginDate.Value.AddDays(i);
        }
        public static IEnumerable<DateTime> GetAllWeekDaysBetweenDates(this DateTime beginDate, DateTime endDate)
        {
            int count = endDate.Subtract(beginDate).Days;
            for (int i = 0; i <= count; i++)
            {
                var date = beginDate.AddDays(i);
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                    yield return date;
            }
        }
        public static DateTime AddWeekdays(this DateTime date, int days)
        {
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                throw new ArgumentException("start must be a weekday");
            int remainder = days % 5;
            int weekendDays = (days / 5) * 2;
            DateTime end = date.AddDays(remainder);
            if (end.DayOfWeek == DayOfWeek.Saturday)
                end = end.AddDays(2);
            else if (end.DayOfWeek < date.DayOfWeek)
                end = end.AddDays(2);
            return end.AddDays(days + weekendDays - remainder);
        }
        //public static DateTime AddWeekdays(this DateTime date, int days)
        //{
        //    var sign = days < 0 ? -1 : 1;
        //    var unsignedDays = Math.Abs(days);
        //    var weekdaysAdded = 0;
        //    while (weekdaysAdded < unsignedDays)
        //    {
        //        date = date.AddDays(sign);
        //        if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
        //            weekdaysAdded++;
        //    }
        //    return date;
        //}
        public static DateTime FirstDayOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }
        public static DateTime LastDayOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
        }
        public static string ToString(this DateTime? date)
        {
            return date.ToString(null, DateTimeFormatInfo.CurrentInfo);
        }
        public static string ToString(this DateTime? date, string format)
        {
            return date.ToString(format, DateTimeFormatInfo.CurrentInfo);
        }
        public static string ToString(this DateTime? date, IFormatProvider provider)
        {
            return date.ToString(null, provider);
        }
        public static string ToString(this DateTime? date, string format, IFormatProvider provider)
        {
            if (date.HasValue)
                return date.Value.ToString(format, provider);
            else
                return string.Empty;
        }
        public static string ToRelativeDateString(this DateTime date)
        {
            return GetRelativeDateValue(date, DateTime.Now);
        }
        public static string ToRelativeDateStringUtc(this DateTime date)
        {
            return GetRelativeDateValue(date, DateTime.UtcNow);
        }
        private static string GetRelativeDateValue(DateTime date, DateTime comparedTo)
        {
            TimeSpan diff = comparedTo.Subtract(date);
            if (diff.TotalDays >= 365)
                return string.Concat("on ", date.ToString("MMMM d, yyyy"));
            if (diff.TotalDays >= 7)
                return string.Concat("on ", date.ToString("MMMM d"));
            else if (diff.TotalDays > 1)
                return string.Format("{0:N0} days ago", diff.TotalDays);
            else if (diff.TotalDays == 1)
                return "yesterday";
            else if (diff.TotalHours >= 2)
                return string.Format("{0:N0} hours ago", diff.TotalHours);
            else if (diff.TotalMinutes >= 60)
                return "more than an hour ago";
            else if (diff.TotalMinutes >= 5)
                return string.Format("{0:N0} minutes ago", diff.TotalMinutes);
            if (diff.TotalMinutes >= 1)
                return "a few minutes ago";
            else
                return "less than a minute ago";
        }
        public static IEnumerable<DateTime> Weeks(this IEnumerable<DateTime> dates)
        {
            return ThoseWeeksMonday(dates).Distinct().ToList();
        } 
        private static IEnumerable<DateTime> ThoseWeeksMonday(IEnumerable<DateTime> dates)
        {
            return dates.Select(d => d.ThisWeekMonday());
        }
        public static string DateTimeToString(this DateTime dateTime)
        {
            return dateTime == DateTime.MinValue
                       ? string.Empty
                       : dateTime.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
        }
    }
}
