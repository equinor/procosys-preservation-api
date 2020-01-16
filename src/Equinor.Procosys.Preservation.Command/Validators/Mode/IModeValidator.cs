namespace Equinor.Procosys.Preservation.Command.Validators.Mode
{
    public interface IModeValidator
    {
        bool Exists(int modeId);

        bool IsVoided(int modeId);
    }
}
