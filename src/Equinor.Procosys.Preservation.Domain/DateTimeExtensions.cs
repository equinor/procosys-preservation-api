using System;

namespace Equinor.Procosys.Preservation.Domain
{
    public static class DateTimeExtensions
    {
        public static DateTime AddWeeks(this DateTime dateTime, int weeks)
            => dateTime.AddDays(7 * weeks);
    }
}
