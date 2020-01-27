using System;

namespace Equinor.Procosys.Preservation.Domain
{
    public interface ITimeService
    {
        DateTime GetCurrentTimeUtc();
    }
}
