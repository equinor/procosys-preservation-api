using System.Collections.Generic;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.RequirementTypes
{
    public class RequirementTypeDetailsDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public RequirementTypeIcon Icon { get; set; }
        public bool IsInUse { get; set; }
        public bool IsVoided { get; set; }
        public int SortKey { get; set; }
        public string RowVersion { get; set; }
        public IEnumerable<RequirementDefinitionDetailDto> RequirementDefinitions { get; set; }
    }
}
