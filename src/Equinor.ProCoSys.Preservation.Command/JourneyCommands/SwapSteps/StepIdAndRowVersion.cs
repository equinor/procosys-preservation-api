namespace Equinor.ProCoSys.Preservation.Command.JourneyCommands.SwapSteps
{
    public class StepIdAndRowVersion
    {
        public StepIdAndRowVersion(int id, string rowVersion)
        {
            Id = id;
            RowVersion = rowVersion;
        }

        public int Id { get; }
        public string RowVersion { get; }
    }
}
