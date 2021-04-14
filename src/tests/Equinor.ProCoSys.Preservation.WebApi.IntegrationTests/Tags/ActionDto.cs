using System;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.Tags
{
    public class ActionDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsOverDue { get; set; }
        public DateTime? DueTimeUtc { get; set; }
        public bool IsClosed { get; set; }
        public int AttachmentCount { get; set; }
        public string RowVersion { get; set; }
    }
}
