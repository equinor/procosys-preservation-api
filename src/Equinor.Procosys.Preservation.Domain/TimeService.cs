using System;

namespace Equinor.Procosys.Preservation.Domain
{
    public static class TimeService
    {
        private static Func<DateTime> s_timeFunc = () => DateTime.UtcNow;

        public static DateTime UtcNow = s_timeFunc();

        public static void Setup(Func<DateTime> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            if (func().Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("Time must be in UTC format");
            }

            s_timeFunc = func;
        }
    }
}
