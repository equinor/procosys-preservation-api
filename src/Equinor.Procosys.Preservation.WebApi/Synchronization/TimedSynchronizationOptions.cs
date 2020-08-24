using System;
using DocumentFormat.OpenXml;

namespace Equinor.Procosys.Preservation.WebApi.Synchronization
{
    public class SynchronizationOptions
    {
        public TimeSpan Interval { get; set; }
        public Guid UserOid { get; set; }
        public bool AutoTransferTags { get; set; } = true;
        public bool SynchronizeProjects { get; set; } = true;
        public bool SynchronizeResponsibles { get; set; } = true;
        public bool SynchronizeTagFunctions { get; set; } = true;
    }
}
