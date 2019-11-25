using System;

namespace Equinor.Procosys.Preservation.Command
{
    public interface ITimeService
    {
        DateTime GetCurrentTimeUTC();
    }
}
