using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    // A field needing input can be of 3 types: Number, CheckBox and Attachment
    // This class represent Number
    public class NumberValue : FieldValue
    {
        protected NumberValue()
        {
        }

        public NumberValue(string schema, Field field, double? value)
            : base(schema, field) => Value = value;

        public double? Value { get; private set; } // Value == null means NA
    }
}
