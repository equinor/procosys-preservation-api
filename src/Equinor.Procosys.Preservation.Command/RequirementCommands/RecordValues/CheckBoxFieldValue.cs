namespace Equinor.ProCoSys.Preservation.Command.RequirementCommands.RecordValues
{
    public class CheckBoxFieldValue
    {
        public CheckBoxFieldValue(int fieldId, bool isChecked)
        {
            FieldId = fieldId;
            IsChecked = isChecked;
        }

        public int FieldId { get; }
        public bool IsChecked { get; }
    }
}
