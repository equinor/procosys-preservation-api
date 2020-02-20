using System;

namespace Equinor.Procosys.Preservation.Query.GetTagActionDetails
{
    public class ActionDetailsDto
    {
        public ActionDetailsDto(
            int id, 
            PersonDto createdBy,
            DateTime createdAt,
            string description,
            DateTime? dueTimeUtc,
            bool isClosed,
            PersonDto closedBy,
            DateTime? closedAtUtc)
        {
            Id = id;
            CreatedBy = createdBy;
            CreatedAt = createdAt;
            Description = description;
            DueTimeUtc = dueTimeUtc;
            IsClosed = isClosed;
            ClosedBy = closedBy;
            ClosedAtUtc = closedAtUtc;
        }


        public int Id { get; }
        public PersonDto CreatedBy { get; }
        public DateTime CreatedAt { get; }
        public string Description { get; }
        public DateTime? DueTimeUtc { get; }
        public bool IsClosed { get; }
        public PersonDto ClosedBy { get; }
        public DateTime? ClosedAtUtc { get; }
    }
}
