using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    // A field needing input can be of 3 types: Number, CheckBox and Attachment
    // This class represent a Number field
    // I.e:
    //      If table row exists -> end user has either filled a numeric value for particular field, or written NA (or N/A) and saved
    //      Value == null in an existing row, represent NA
    //      If end user blank an existing value (or NA) for particular field, and save, table row will be deleted
    public class NumberValue : FieldValue
    {
        protected NumberValue()
        {
        }

        public NumberValue(string plant, Field field, double? value)
            : base(plant, field) =>
            Value = value;

        public double? Value { get; private set; }
    }
}
