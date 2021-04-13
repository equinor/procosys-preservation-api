using System.Collections.Generic;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.RequirementTypes
{
    public class RequirementDefinitionDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsVoided { get; set; }
        public int DefaultIntervalWeeks { get; set; }
        public RequirementUsage Usage { get; set; }
        public int SortKey { get; set; }
        public bool NeedsUserInput { get; set; }
        public IEnumerable<FieldDetailsDto> Fields { get; set; }
        public string RowVersion { get; set;  }
    }
}
