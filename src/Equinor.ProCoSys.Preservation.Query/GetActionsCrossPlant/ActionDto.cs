using System;

namespace Equinor.ProCoSys.Preservation.Query.GetActionsCrossPlant
{
    public class ActionDto
    {
        public ActionDto(
            string plantId,
            string plantTitle,
            string projectName,
            string projectDescription,
            bool projectIsClosed,
            int tagId,
            string tagNo,
            int id,
            string title,
            string description,
            bool isOverDue,
            DateTime? dueTimeUtc,
            bool isClosed,
            int attachmentCount)
        {
            PlantId = plantId;
            PlantTitle = plantTitle;
            ProjectName = projectName;
            ProjectDescription = projectDescription;
            ProjectIsClosed = projectIsClosed;
            TagId = tagId;
            TagNo = tagNo;
            Id = id;
            Title = title;
            Description = description;
            IsOverDue = isOverDue;
            DueTimeUtc = dueTimeUtc;
            IsClosed = isClosed;
            AttachmentCount = attachmentCount;
        }

        public string PlantId { get; }
        public string PlantTitle { get; }
        public string ProjectName { get; }
        public string ProjectDescription { get; }
        public bool ProjectIsClosed { get; }
        public int TagId { get; }
        public string TagNo { get; }
        public int Id { get; }
        public string Title { get; }
        public string Description { get; }
        public bool IsOverDue { get; }
        public DateTime? DueTimeUtc { get; }
        public bool IsClosed { get; }
        public int AttachmentCount { get; }
    }
}
