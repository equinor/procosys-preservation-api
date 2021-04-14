using System;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.Tags
{
    public class RequirementDto
    {
        public int Id { get; set; }
        public string RequirementTypeCode { get; set; }
        public RequirementTypeIcon RequirementTypeIcon { get; set; }
        public DateTime? NextDueTimeUtc { get; set; }
        public string NextDueAsYearAndWeek { get; set; }
        public int? NextDueWeeks { get; set; }
        public bool ReadyToBePreserved { get; set; }
    }
}
