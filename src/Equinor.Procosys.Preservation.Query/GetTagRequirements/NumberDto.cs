using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.Procosys.Preservation.Query.GetTagRequirements
{
    public class NumberDto
    {
        public NumberDto(NumberValue fieldValue)
        {
            IsNA = !fieldValue.Value.HasValue;
            Value = fieldValue.Value;
        }

        public bool IsNA { get; }
        public double? Value { get; set; }
    }
}
