using System;
using System.Globalization;
using System.Threading;

namespace Equinor.Procosys.Preservation.Domain
{
    public static class DateTimeExtensions
    {
        private static DayOfWeek firstDayOfWeek = Thread.CurrentThread.CurrentCulture.DateTimeFormat.FirstDayOfWeek;

        public static DateTime AddWeeks(this DateTime dateTime, int weeks)
            => dateTime.AddDays(7 * weeks);

        public static string FormatAsYearAndWeekString(this DateTime dateTime)
            => string.Concat(ISOWeek.GetYear(dateTime).ToString(), "w", ISOWeek.GetWeekOfYear(dateTime).ToString("00"));

        public static DateTime StartOfWeek(this DateTime dt)
        {
            var dayOfWeek = GetDayOfWeekMondayAsFirst(dt);
            var diff = -(dayOfWeek - (int)firstDayOfWeek);
            return dt.AddDays(diff).Date;

        }

        public static int GetWeeksReferencedFromStartOfWeek(this DateTime toDateTime, DateTime fromDateTime)
        {
            var startOfFromWeek = fromDateTime.StartOfWeek();
            var startOfToWeek = toDateTime.StartOfWeek();
            var timeSpan = startOfToWeek - startOfFromWeek;
            
            return timeSpan.Weeks();
        }

        private static int GetDayOfWeekMondayAsFirst(DateTime dt)
        {
            if (dt.DayOfWeek == DayOfWeek.Sunday)
            {
                // treat Sunday as last day of week since enum DayOfWeek has Sunday as 0
                return 7;
            }

            return (int)dt.DayOfWeek;
        }
    }
}
