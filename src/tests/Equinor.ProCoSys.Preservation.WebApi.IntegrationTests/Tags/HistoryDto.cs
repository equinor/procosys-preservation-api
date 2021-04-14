using System;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.HistoryAggregate;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.Tags
{
    public class HistoryDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public PersonDto CreatedBy { get; set; }
        public EventType EventType { get; set; }
        public int? DueWeeks { get; set; }
        public int? TagRequirementId { get; set; }
        public Guid? PreservationRecordGuid { get; set; }
    }
}
