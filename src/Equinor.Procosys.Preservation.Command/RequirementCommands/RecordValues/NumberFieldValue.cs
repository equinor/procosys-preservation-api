namespace Equinor.Procosys.Preservation.Command.RequirementCommands.RecordValues
{
    public class NumberFieldValue
    {
        public NumberFieldValue(int fieldId, double? value, bool isNa)
        {
            FieldId = fieldId;
            Value = value;
            IsNa = isNa;
        }

        public int FieldId { get; }
        public double? Value { get; }
        public bool IsNa { get; }
    }
}
