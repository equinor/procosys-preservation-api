namespace Equinor.Procosys.Preservation.Command.TagCommands.RecordCommands.RecordCheckBoxChecked
{
    public class RecordCheckBoxCheckedCommand : RecordCommand
    {
        public RecordCheckBoxCheckedCommand(int tagId, int fieldId, bool value) : base(tagId, fieldId)
            => Value = value;

        public bool Value { get; }
    }
}
