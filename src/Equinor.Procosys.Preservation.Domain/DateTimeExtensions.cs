using System;
using System.Globalization;

namespace Equinor.Procosys.Preservation.Domain
{
    public static class DateTimeExtensions
    {
        public static DateTime AddWeeks(this DateTime dateTime, int weeks)
            => dateTime.AddDays(7 * weeks);

        public static string FormatAsYearAndWeekString(this DateTime dateTime)
            => string.Concat(ISOWeek.GetYear(dateTime).ToString(), "w", ISOWeek.GetWeekOfYear(dateTime).ToString("00"));
    }
}
