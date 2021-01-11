using System;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Tags
{
    public class ActionDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime? DueTimeUtc { get; set;  }
        public string Description { get; set; }
        public bool IsClosed { get; set; }
        public DateTime? ClosedAtUtc { get; set; }
        public int AttachmentCount { get; set; }
        public DateTime? ModifiedAtUtc { get; set; }
        public string RowVersion { get; set; }
    }
}
