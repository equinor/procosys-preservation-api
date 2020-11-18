using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.RequirementTypes
{
    public class RequirementTypeDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public RequirementTypeIcon Icon { get; set; }
        public bool IsVoided { get; set; }
        public int SortKey { get; set; }
        public string RowVersion { get; set; }
        public IEnumerable<RequirementDefinitionDto> RequirementDefinitions { get; set; }
    }
}
