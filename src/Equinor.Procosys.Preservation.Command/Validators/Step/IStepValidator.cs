namespace Equinor.Procosys.Preservation.Command.Validators.Step
{
    public interface IStepValidator
    {
        bool Exists(int stepId);

        bool IsVoided(int stepId);
    }
}
