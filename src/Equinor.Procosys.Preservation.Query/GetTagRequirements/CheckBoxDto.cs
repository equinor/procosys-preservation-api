using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.Procosys.Preservation.Query.GetTagRequirements
{
    public class CheckBoxDto
    {
        public CheckBoxDto() => IsChecked = true;

        public bool IsChecked { get; }
    }
}
