namespace Equinor.Procosys.Preservation.Command.TagCommands.CreateTag
{
    public class Requirement
    {
        public Requirement(int requirementDefinitionId, int intervalWeeks)
        {
            RequirementDefinitionId = requirementDefinitionId;
            IntervalWeeks = intervalWeeks;
        }

        public int RequirementDefinitionId { get;  }
        public int IntervalWeeks { get; }
    }
}
