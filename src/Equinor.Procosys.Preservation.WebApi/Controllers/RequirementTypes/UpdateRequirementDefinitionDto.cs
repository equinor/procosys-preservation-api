using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.RequirementTypes
{
    public class UpdateRequirementDefinitionDto
    {
        public int SortKey { get; set; }
        public RequirementUsage Usage { get; set; }
        public string Title { get; set; }
        public int DefaultIntervalWeeks { get; set; }
        public string RowVersion { get; set; }
        public IList<FieldDto> UpdatedFields { get; set; }
        public IList<FieldDto> NewFields { get; set; }
    }
}
