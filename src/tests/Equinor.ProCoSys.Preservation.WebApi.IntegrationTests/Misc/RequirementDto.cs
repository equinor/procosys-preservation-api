using System;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.Misc
{
    public class RequirementDto
    {
        public int Id { get; set; }
        public string RequirementTypeCode { get; set; }
        public string RequirementDefinitionTitle { get; set; }
        public DateTime? NextDueTimeUtc { get; set; }
        public string NextDueAsYearAndWeek { get; set; }
        public int? NextDueWeeks { get; set; }
        public bool ReadyToBePreserved { get; set; }
    }
}
