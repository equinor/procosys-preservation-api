using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.HistoryAggregate;

namespace Equinor.Procosys.Preservation.Query.GetHistory
{
    public class HistoryDto
    {
        public HistoryDto(
            int id,
            string description,
            DateTime createdAtUtc,
            PersonDto createdBy,
            EventType eventType,
            int? dueWeeks,
            int? tagRequirementId,
            Guid? preservationRecordGuid)
        {
            Id = id;
            Description = description;
            CreatedBy = createdBy;
            CreatedAtUtc = createdAtUtc;
            EventType = eventType;
            DueWeeks = dueWeeks;
            TagRequirementId = tagRequirementId;
            PreservationRecordGuid = preservationRecordGuid;
        }

        public int Id { get; }
        public string Description { get; }
        public DateTime CreatedAtUtc { get; }
        public PersonDto CreatedBy { get; }
        public EventType EventType { get; }
        public int? DueWeeks { get; }
        public int? TagRequirementId { get; }
        public Guid? PreservationRecordGuid { get; }
    }
}
