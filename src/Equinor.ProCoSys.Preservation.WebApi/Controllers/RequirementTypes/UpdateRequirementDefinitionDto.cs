using System.Collections.Generic;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.WebApi.Controllers.RequirementTypes
{
    public class UpdateRequirementDefinitionDto
    {
        public int SortKey { get; set; }
        public RequirementUsage Usage { get; set; }
        public string Title { get; set; }
        public int DefaultIntervalWeeks { get; set; }
        public string RowVersion { get; set; }
        // Existing fields not included in UpdatedFields will be deleted. These must be Voided in advance, and they can't be in use
        public IList<UpdateFieldDto> UpdatedFields { get; set; }
        public IList<FieldDto> NewFields { get; set; }
    }
}
