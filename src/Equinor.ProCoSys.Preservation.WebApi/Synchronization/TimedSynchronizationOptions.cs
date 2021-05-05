using System;

namespace Equinor.ProCoSys.Preservation.WebApi.Synchronization
{
    public class SynchronizationOptions
    {
        public TimeSpan Interval { get; set; }
        public Guid UserOid { get; set; }
        public bool AutoTransferTags { get; set; } = true;
    }
}
