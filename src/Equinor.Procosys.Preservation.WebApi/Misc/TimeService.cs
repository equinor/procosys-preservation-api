using Equinor.Procosys.Preservation.Command;
using System;

namespace Equinor.Procosys.Preservation.WebApi.Misc
{
    public class TimeService : ITimeService
    {
        public DateTime GetCurrentTimeUTC()
        {
            return DateTime.UtcNow;
        }
    }
}
