using System;

namespace Equinor.Procosys.Preservation.Domain
{
    public static class TimeSpanExtensions
    {
        public static int Weeks(this TimeSpan span) => span.Days / 7;
    }
}
