namespace Equinor.Procosys.Preservation.Command.Validators.Mode
{
    public interface IModeValidator
    {
        bool Exists(int modeId);

        bool Exists(string title);
        bool IsVoided(int modeId);
    }
}
