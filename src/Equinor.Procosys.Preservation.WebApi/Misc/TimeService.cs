using System;
using Equinor.Procosys.Preservation.Command;

namespace Equinor.Procosys.Preservation.WebApi.Misc
{
    public class TimeService : ITimeService
    {
        public DateTime GetCurrentTimeUTC() => DateTime.UtcNow;
    }
}
