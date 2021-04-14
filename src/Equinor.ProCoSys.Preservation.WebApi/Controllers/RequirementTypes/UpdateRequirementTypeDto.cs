using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.WebApi.Controllers.RequirementTypes
{
    public class UpdateRequirementTypeDto
    {
        public int SortKey { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public RequirementTypeIcon Icon { get; set; }
        public string RowVersion { get; set; }
    }
}
