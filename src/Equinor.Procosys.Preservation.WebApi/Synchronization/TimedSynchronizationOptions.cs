using System;

namespace Equinor.Procosys.Preservation.WebApi.Synchronization
{
    public class SynchronizationOptions
    {
        public TimeSpan Interval { get; set; }
        public Guid UserOid { get; set; }
    }
}
