﻿namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.Tags
{
    public class UpdatedTagRequirementDto
    {
        public int RequirementId { get; set; }
        public int IntervalWeeks { get; set; }
        public bool IsVoided { get; set; }
        public string RowVersion { get; set; }
    }
}
