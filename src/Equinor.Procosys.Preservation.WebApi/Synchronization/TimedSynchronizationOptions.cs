using System;

namespace Equinor.Procosys.Preservation.WebApi.Synchronization
{
    public class SynchronizationOptions
    {
        public TimeSpan StartupDelay { get; set; }
        public TimeSpan Interval { get; set; }
    }
}
