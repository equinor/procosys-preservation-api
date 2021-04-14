using System;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Query.GetTagRequirements
{
    public class NumberDetailsDto
    {
        public NumberDetailsDto(NumberValue fieldValue)
        {
            if (fieldValue == null)
            {
                throw new ArgumentNullException(nameof(fieldValue));
            }

            IsNA = !fieldValue.Value.HasValue;
            Value = fieldValue.Value;
        }

        public bool IsNA { get; }
        public double? Value { get; set; }
    }
}
