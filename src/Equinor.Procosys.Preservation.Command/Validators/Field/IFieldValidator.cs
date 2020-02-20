namespace Equinor.Procosys.Preservation.Command.Validators.Field
{
    public interface IFieldValidator
    {
        bool Exists(int fieldId);
        
        bool IsVoided(int fieldId);

        bool IsValidValue(int fieldId, string value);
        
        bool IsValidForRecording(int fieldId);
    }
}
