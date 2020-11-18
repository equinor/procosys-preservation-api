using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.RequirementTypes
{
    public class CreateRequirementDefinitionDto
    {
        public int SortKey { get; set; }
        public RequirementUsage Usage { get; set; }
        public string Title { get; set; }
        public int DefaultIntervalWeeks { get; set; }
        public IList<FieldDto> Fields { get; set; } = new List<FieldDto>();
    }
}
