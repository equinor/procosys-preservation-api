using System;
using Equinor.Procosys.Preservation.Domain;

namespace Equinor.Procosys.Preservation.WebApi.Misc
{
    public class TimeService : ITimeService
    {
        public DateTime GetCurrentTimeUtc() => DateTime.UtcNow;
    }
}
