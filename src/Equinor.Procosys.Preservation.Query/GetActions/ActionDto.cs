using System;

namespace Equinor.Procosys.Preservation.Query.GetActions
{
    public class ActionDto
    {
        public ActionDto(
            int id,
            string title,
            bool isOverDue,
            DateTime? dueTimeUtc,
            bool isClosed,
            int attachmentCount,
            string rowVersion)
        {
            Id = id;
            Title = title;
            IsOverDue = isOverDue;
            DueTimeUtc = dueTimeUtc;
            IsClosed = isClosed;
            AttachmentCount = attachmentCount;
            RowVersion = rowVersion;
        }

        public int Id { get; }
        public string Title { get; }
        public bool IsOverDue { get; }
        public DateTime? DueTimeUtc { get; }
        public bool IsClosed { get; }
        public int AttachmentCount { get; }
        public string RowVersion { get; }
    }
}
