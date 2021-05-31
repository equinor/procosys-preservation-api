using System;

namespace Equinor.ProCoSys.Preservation.Query.GetAllActionsCrossPlant
{
    public class ActionDto
    {
        public ActionDto(
            string plant,
            string plantDescription,
            string projectName,
            string projectDescription,
            int tagId,
            string tagNo,
            int id,
            string title,
            bool isOverDue,
            DateTime? dueTimeUtc,
            bool isClosed,
            int attachmentCount)
        {
            Plant = plant;
            PlantDescription = plantDescription;
            ProjectName = projectName;
            ProjectDescription = projectDescription;
            TagId = tagId;
            TagNo = tagNo;
            Id = id;
            Title = title;
            IsOverDue = isOverDue;
            DueTimeUtc = dueTimeUtc;
            IsClosed = isClosed;
            AttachmentCount = attachmentCount;
        }

        public string Plant { get; }
        public string PlantDescription { get; }
        public string ProjectName { get; }
        public string ProjectDescription { get; }
        public int TagId { get; }
        public string TagNo { get; }
        public int Id { get; }
        public string Title { get; }
        public bool IsOverDue { get; }
        public DateTime? DueTimeUtc { get; }
        public bool IsClosed { get; }
        public int AttachmentCount { get; }
    }
}
