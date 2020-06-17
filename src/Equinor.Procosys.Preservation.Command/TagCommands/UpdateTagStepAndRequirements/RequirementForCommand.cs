namespace Equinor.Procosys.Preservation.Command.TagCommands.UpdateTagStepAndRequirements
{
    public class UpdateRequirementForCommand
    {
        public UpdateRequirementForCommand(int requirementId, int intervalWeeks, bool isVoided, string rowVersion)
        {
            RequirementId = requirementId;
            IntervalWeeks = intervalWeeks;
            IsVoided = isVoided;
            RowVersion = rowVersion;
        }

        public int RequirementId { get;  }
        public int IntervalWeeks { get; }
        public bool IsVoided { get; }
        public string RowVersion { get; set; }
    }
}
