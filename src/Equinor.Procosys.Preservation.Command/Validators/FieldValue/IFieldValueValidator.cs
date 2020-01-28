namespace Equinor.Procosys.Preservation.Command.Validators.FieldValue
{
    public interface IFieldValueValidator
    {
        bool ExistsInCurrentPeriod(int fieldId);
    }
}
