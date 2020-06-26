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
            int? preservationRecordId)
        {
            Id = id;
            Description = description;
            CreatedBy = createdBy;
            CreatedAtUtc = createdAtUtc;
            EventType = eventType;
            DueWeeks = dueWeeks;
            PreservationRecordId = preservationRecordId;
        }

        public int Id { get; }
        public string Description { get; }
        public DateTime CreatedAtUtc { get; }
        public PersonDto CreatedBy { get; }
        public EventType EventType { get; }
        public int? DueWeeks { get; }
        public int? PreservationRecordId { get; }
    }
}
