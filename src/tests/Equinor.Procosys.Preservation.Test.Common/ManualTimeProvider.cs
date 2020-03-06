using System;
using Equinor.Procosys.Preservation.Domain.Time;

namespace Equinor.Procosys.Preservation.Test.Common
{
    public class ManualTimeProvider : ITimeProvider
    {
        public ManualTimeProvider()
        {
        }

        public ManualTimeProvider(DateTime now)
        {
            if (now.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("Must be UTC");
            }

            UtcNow = now;
        }

        public DateTime UtcNow { get; set; }

        public void Elapse(TimeSpan elapsedTime) => UtcNow += elapsedTime;
    }
}
