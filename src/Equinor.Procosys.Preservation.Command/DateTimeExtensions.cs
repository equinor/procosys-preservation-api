using System;

namespace Equinor.Procosys.Preservation.Command
{
    public static class DateTimeExtensions
    {
        public static DateTime AddWeeks(this DateTime dateTime, int weeks)
            => dateTime.AddDays(7 * weeks);
    }
}
