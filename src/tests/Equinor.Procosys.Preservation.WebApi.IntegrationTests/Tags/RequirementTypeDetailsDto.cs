using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Tags
{
    public class RequirementTypeDetailsDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public RequirementTypeIcon Icon { get; set; }
        public string Title { get; set; }
    }
}
