using System;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.Misc
{
    public class ActionDto
    {
        public string PlantId { get; set; }
        public string PlantTitle { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public int TagId { get; set; }
        public string TagNo { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsOverDue { get; set; }
        public DateTime? DueTimeUtc { get; set; }
        public bool IsClosed { get; set; }
        public DateTime? ClosedTimeUtc { get; set; }
        public int AttachmentCount { get; set; }
    }
}
