namespace Equinor.Procosys.Preservation.Command.TagCommands.RecordValues
{
    public class FieldValue
    {
        public FieldValue(int fieldId, string value)
        {
            FieldId = fieldId;
            Value = value;
        }

        public int FieldId { get; }
        public string Value { get; }
    }
}
