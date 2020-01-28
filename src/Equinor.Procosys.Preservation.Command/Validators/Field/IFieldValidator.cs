namespace Equinor.Procosys.Preservation.Command.Validators.Field
{
    public interface IFieldValidator
    {
        bool Exists(int fieldId);
        bool IsVoided(int fieldId);
    }
}
