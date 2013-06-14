using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;

namespace Utilities.Extensions
{
    public static class DateTimeExtension
    {
        public static DateTime TodayOrPrevMonday(this DateTime date)
        {
            return TodayOrPrev(date, DayOfWeek.Monday);
        }

        public static DateTime TodayOrPrevTuesday(this DateTime date)
        {
            return TodayOrPrev(date, DayOfWeek.Tuesday);
        }

        public static DateTime TodayOrPrevWednesday(this DateTime date)
        {
            return TodayOrPrev(date, DayOfWeek.Wednesday);
        }

        public static DateTime TodayOrPrevThursday(this DateTime date)
        {
            return TodayOrPrev(date, DayOfWeek.Thursday);
        }

        public static DateTime TodayOrPrevFriday(this DateTime date)
        {
            return TodayOrPrev(date, DayOfWeek.Friday);
        }

        public static DateTime TodayOrPrevSaturday(this DateTime date)
        {
            return TodayOrPrev(date, DayOfWeek.Saturday);
        }

        public static DateTime TodayOrPrevSunday(this DateTime date)
        {
            return TodayOrPrev(date, DayOfWeek.Sunday);
        }

        private static DateTime TodayOrPrev(DateTime date, DayOfWeek day)
        {
            int diff = (int)day - (int)date.DayOfWeek;
            return date.AddDays(diff <= 0 ? diff : diff - 7);
        }

        public static string ToDevExpressString(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        public static DateTime ThisWeekMonday(this DateTime date)
        {
            return date.AddDays((int)DayOfWeek.Monday - (int)date.DayOfWeek);
        }

        public static DateTime? ThisWeekMonday(this DateTime? date)
        {
            if (date == null)
                return date;
            return date.Value.AddDays((int)DayOfWeek.Monday - (int)date.Value.DayOfWeek);
        }

        public static DateTime ThisWeekTuesday(this DateTime date)
        {
            return date.AddDays((int)DayOfWeek.Tuesday - (int)date.DayOfWeek);
        }

        public static DateTime ThisWeekWednesday(this DateTime date)
        {
            return date.AddDays((int)DayOfWeek.Wednesday - (int)date.DayOfWeek);
        }

        public static DateTime ThisWeekThursday(this DateTime date)
        {
            return date.AddDays((int)DayOfWeek.Thursday - (int)date.DayOfWeek);
        }

        public static DateTime ThisWeekFriday(this DateTime date)
        {
            return date.AddDays((int)DayOfWeek.Friday - (int)date.DayOfWeek);
        }

        public static DateTime ThisWeekSaturday(this DateTime date)
        {
            return date.AddDays((int)DayOfWeek.Saturday - (int)date.DayOfWeek);
        }

        public static DateTime ThisWeekSunday(this DateTime date)
        {
            return date.AddDays((int)DayOfWeek.Sunday - (int)date.DayOfWeek);
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
            return date.AddDays((7 + (int)DayOfWeek.Monday) - (int)date.DayOfWeek);
        }

        public static DateTime PrevFriday(this DateTime date)
        {
            return date.AddDays(0 - ((7 - (int)DayOfWeek.Friday) + (int)date.DayOfWeek));
        }

        public static IEnumerable<DateTime?> GetAllDaysBetweenDates(this DateTime? beginDate, DateTime? endDate)
        {
            Contract.Requires(beginDate != null);
            Contract.Requires(endDate != null);

            int count = endDate.Value.Subtract(beginDate.Value).Days;
            for (int i = 0; i <= count; i++)
                yield return beginDate.Value.AddDays(i);
        }

        public static IEnumerable<DateTime> GetAllDaysBetweenDates(this DateTime beginDate, DateTime endDate)
        {
            int count = endDate.Subtract(beginDate).Days;
            for (int i = 0; i <= count; i++)
                yield return beginDate.AddDays(i);
        }

        public static IEnumerable<DateTime> GetAllWeekDaysBetweenDates(this DateTime beginDate, DateTime endDate)
        {
            int count = endDate.Subtract(beginDate).Days;
            for (int i = 0; i <= count; i++)
            {
                DateTime date = beginDate.AddDays(i);
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                    yield return date;
            }
        }

        public static DateTime AddWeekdays(this DateTime date, int days)
        {
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) throw new ArgumentException("start must be a weekday");
            Contract.EndContractBlock();

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
            Contract.Requires(dates != null);

            return ThoseWeeksMonday(dates).Distinct().ToList();
        }

        private static IEnumerable<DateTime> ThoseWeeksMonday(IEnumerable<DateTime> dates)
        {
            Contract.Requires(dates != null);

            return dates.Select(d => d.ThisWeekMonday());
        }

        public static int MonthDifference(this DateTime startDate, DateTime endDate)
        {
            return Math.Abs(12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month);
        }

        public static int YearDifference(this DateTime startDate, DateTime endDate)
        {
            return startDate.Year - endDate.Year;
        }

        public static DateTime AddWeeks(this DateTime date)
        {
            DateTime nextWeek = date.AddDays(7);
            if (nextWeek.Month == date.Month)
                return nextWeek;
            return date;
        }

        public static DateTime? GetDayOfWeekInTheMonth(this DateTime date, DayOfWeek dayOfWeek, int instance)
        {
            if (instance <= 0)
                return null;

            date = FirstDayOfTheMonth(date, dayOfWeek);

            if (instance == 1)
                return date;
            return date.AddWeeks();
        }

        private static DateTime FirstDayOfTheMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1, date.Hour, date.Minute, date.Second);
        }

        private static DateTime FirstDayOfTheMonth(DateTime date, DayOfWeek dayOfWeek)
        {
            DateTime firstDay = FirstDayOfTheMonth(date);
            if ((int)dayOfWeek - (int)firstDay.DayOfWeek > 0)
                return date.AddDays((int)dayOfWeek - (int)firstDay.DayOfWeek);
            return date.AddDays((7 + (int)dayOfWeek) - (int)date.DayOfWeek);
        }

        public static DateTime AddYearsAndManageLeapYear(this DateTime date, int value)
        {
            Contract.Requires(value >= 1 && value <= 9999);

            int currentYear = date.Year;
            int diff = value - currentYear;
            bool currentYearIsLeapYear = DateTime.IsLeapYear(currentYear);
            bool newYearIsLeapYear = DateTime.IsLeapYear(value);

            return !currentYearIsLeapYear && newYearIsLeapYear && date.Day == 28 && date.Month == 2 ? date.AddYears(diff).AddDays(1) : date.AddYears(diff);
        }

        public static bool IsYearValid(this int year)
        {
            return year >= DateTime.MinValue.Year && year <= DateTime.MaxValue.Year;
        }

        public static string GetTrimestre(this DateTime date)
        {
            if (date.Month >= 1 && date.Month <= 3) return "T1";
            if (date.Month >= 4 && date.Month <= 6) return "T2";
            if (date.Month >= 7 && date.Month <= 9) return "T3";
            if (date.Month >= 10 && date.Month <= 12) return "T4";
            return null;
        }
    }
}