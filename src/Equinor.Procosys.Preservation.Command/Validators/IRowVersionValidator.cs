namespace Equinor.Procosys.Preservation.Command.Validators
{
    public interface IRowVersionValidator
    {
        bool IsValid(string rowVersion);
    }
}
