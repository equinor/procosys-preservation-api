using System;
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

        public NumberValue(string schema, Field field, string value)
            : base(schema, field)
        {
            if (!IsValidValue(value, out var number))
            {
                throw new ArgumentException($"Value {value} is not a legal value for a {nameof(NumberValue)}");
            }

            Value = number;
        }

        public double? Value { get; private set; }

        public static bool IsValidValue(string value, out double? number)
        {
            number = null;
            if (value.ToUpper() == "NA" || value.ToUpper() == "N/A")
            {
                return true;
            }

            if (double.TryParse(value, out var n))
            {
                number = n;
                return true;
            }

            return false;
        }
    }
}
