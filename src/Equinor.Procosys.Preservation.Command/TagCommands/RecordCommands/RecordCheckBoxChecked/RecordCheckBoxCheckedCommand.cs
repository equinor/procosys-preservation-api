namespace Equinor.Procosys.Preservation.Command.TagCommands.RecordCommands.RecordCheckBoxChecked
{
    public class RecordCheckBoxCheckedCommand : RecordCommand
    {
        public RecordCheckBoxCheckedCommand(int tagId, int fieldId, bool isChecked) : base(tagId, fieldId)
            => IsChecked = isChecked;

        public bool IsChecked { get; }
    }
}
