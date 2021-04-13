namespace Equinor.ProCoSys.Preservation.Command.Validators
{
    public interface IRowVersionValidator
    {
        bool IsValid(string rowVersion);
    }
}
