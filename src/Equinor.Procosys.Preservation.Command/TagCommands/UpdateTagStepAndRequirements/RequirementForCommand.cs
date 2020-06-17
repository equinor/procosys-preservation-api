namespace Equinor.Procosys.Preservation.Command.TagCommands.UpdateTagStepAndRequirements
{
    public class UpdateRequirementForCommand
    {
        public UpdateRequirementForCommand(int requirementDefinitionId, int intervalWeeks, bool isVoided)
        {
            RequirementDefinitionId = requirementDefinitionId;
            IntervalWeeks = intervalWeeks;
            IsVoided = isVoided;
        }

        public int RequirementDefinitionId { get;  }
        public int IntervalWeeks { get; }
        public bool IsVoided { get; }
    }
}
