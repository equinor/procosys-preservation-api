using System;
using System.Collections.Generic;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.Tags
{
    public class RequirementDetailsDto
    {
        public int Id { get; set; }
        public int IntervalWeeks { get; set; }
        public int? NextDueWeeks { get; set; }
        public RequirementTypeDetailsDto RequirementType { get; set; }
        public RequirementDefinitionDetailDto RequirementDefinition { get; set; }
        public DateTime? NextDueTimeUtc { get; set; }
        public string NextDueAsYearAndWeek { get; set; }
        public bool ReadyToBePreserved { get; set; }
        public List<FieldDetailsDto> Fields { get; set; }
        public string Comment { get; set; }
        public bool IsVoided { get;  set; }
        public string RowVersion { get; set; }
    }
}
