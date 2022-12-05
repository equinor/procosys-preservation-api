using System;

namespace Equinor.ProCoSys.Preservation.WebApi.Synchronization
{
    public class SynchronizationOptions
    {
        public TimeSpan Interval { get; set; }
        public bool DryRun { get; set; }
        public bool Enabled { get; set; }
    }
}
