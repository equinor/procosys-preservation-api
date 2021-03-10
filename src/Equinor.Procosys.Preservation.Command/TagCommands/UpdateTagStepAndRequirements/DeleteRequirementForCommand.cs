namespace Equinor.Procosys.Preservation.Command.TagCommands.UpdateTagStepAndRequirements
{
    public class DeleteRequirementForCommand
    {
        public DeleteRequirementForCommand(int tagRequirementId, string rowVersion)
        {
            TagRequirementId = tagRequirementId;
            RowVersion = rowVersion;
        }

        public int TagRequirementId { get;  }
        public string RowVersion { get; set; }
    }
}
