using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.WebApi.Controllers.RequirementTypes
{
    public class CreateRequirementTypeDto
    {
        public int SortKey { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public RequirementTypeIcon Icon { get; set; }
    }
}
