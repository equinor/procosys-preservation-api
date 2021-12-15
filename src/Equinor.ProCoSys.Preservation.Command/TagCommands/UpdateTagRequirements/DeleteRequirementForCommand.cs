namespace Equinor.ProCoSys.Preservation.Command.TagCommands.UpdateTagRequirements
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
