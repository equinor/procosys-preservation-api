namespace Equinor.Procosys.Preservation.Command.Validators.Journey
{
    public interface IJourneyValidator
    {
        bool Exists(int journeyId);
        bool Exists(string title);
        bool IsVoided(int journeyId);
    }
}
