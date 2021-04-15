using System;
using System.Globalization;

namespace Equinor.ProCoSys.Preservation.Domain
{
    public static class DateTimeExtensions
    {
        private static readonly DayOfWeek firstDayOfPreservationWeek = DayOfWeek.Monday;

        public static DateTime AddWeeks(this DateTime dateTime, int weeks)
            => dateTime.AddDays(7 * weeks);

        public static string FormatAsYearAndWeekString(this DateTime dateTime)
            => string.Concat(ISOWeek.GetYear(dateTime).ToString(), "w", ISOWeek.GetWeekOfYear(dateTime).ToString("00"));

        public static DateTime StartOfPreservationWeek(this DateTime dt)
        {
            var dayOfWeek = GetDayOfWeekMondayAsFirst(dt);
            var diff = -(dayOfWeek - (int)firstDayOfPreservationWeek);
            return dt.AddDays(diff).Date;
        }

        public static int GetWeeksUntil(this DateTime fromDateTime, DateTime toDateTime)
        {
            var startOfFromWeek = fromDateTime.StartOfPreservationWeek();
            var startOfToWeek = toDateTime.StartOfPreservationWeek();
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
