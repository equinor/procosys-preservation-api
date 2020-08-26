using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.Procosys.Preservation.Query.GetTagRequirements
{
    public class RequirementTypeDetailsDto
    {
        public RequirementTypeDetailsDto(int id, string code, RequirementTypeIcon icon, string title)
        {
            Id = id;
            Code = code;
            Icon = icon;
            Title = title;
        }

        public int Id { get; }
        public string Code { get; }
        public RequirementTypeIcon Icon { get; }
        public string Title { get; }
    }
}
