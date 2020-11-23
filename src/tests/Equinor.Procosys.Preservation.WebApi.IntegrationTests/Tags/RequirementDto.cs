using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Tags
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
