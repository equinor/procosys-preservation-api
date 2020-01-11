namespace Equinor.Procosys.Preservation.Command.TagCommands.CreateTag
{
    public class RequirementDto
    {
        public RequirementDto(int requirementDefinitionId, int interval)
        {
            RequirementDefinitionId = requirementDefinitionId;
            Interval = interval;
        }

        public int RequirementDefinitionId { get;  }
        public int Interval { get; }
    }
}
