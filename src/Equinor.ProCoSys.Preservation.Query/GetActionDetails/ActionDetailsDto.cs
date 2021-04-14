using System;

namespace Equinor.ProCoSys.Preservation.Query.GetActionDetails
{
    public class ActionDetailsDto
    {
        public ActionDetailsDto(
            int id,
            PersonDto createdBy,
            DateTime createdAt,
            string title,
            string description,
            DateTime? dueTimeUtc,
            bool isClosed,
            PersonDto closedBy,
            DateTime? closedAtUtc,
            int attachmentCount,
            PersonDto modifiedBy,
            DateTime? modifiedAt,
            string rowVersion)
        {
            Id = id;
            CreatedBy = createdBy;
            CreatedAtUtc = createdAt;
            Title = title;
            Description = description;
            DueTimeUtc = dueTimeUtc;
            IsClosed = isClosed;
            ClosedBy = closedBy;
            ClosedAtUtc = closedAtUtc;
            AttachmentCount = attachmentCount;
            ModifiedBy = modifiedBy;
            ModifiedAtUtc = modifiedAt;
            RowVersion = rowVersion;
        }

        public int Id { get; }
        public PersonDto CreatedBy { get; }
        public DateTime CreatedAtUtc { get; }
        public string Title { get; }
        public string Description { get; }
        public DateTime? DueTimeUtc { get; }
        public bool IsClosed { get; }
        public PersonDto ClosedBy { get; }
        public DateTime? ClosedAtUtc { get; }
        public int AttachmentCount { get; }
        public PersonDto ModifiedBy { get; }
        public DateTime? ModifiedAtUtc { get; }
        public string RowVersion { get; }
    }
}
