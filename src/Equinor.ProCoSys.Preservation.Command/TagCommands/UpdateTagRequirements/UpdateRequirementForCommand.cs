namespace Equinor.ProCoSys.Preservation.Command.TagCommands.UpdateTagRequirements
{
    public class UpdateRequirementForCommand
    {
        public UpdateRequirementForCommand(int tagRequirementId, int intervalWeeks, bool isVoided, string rowVersion)
        {
            TagRequirementId = tagRequirementId;
            IntervalWeeks = intervalWeeks;
            IsVoided = isVoided;
            RowVersion = rowVersion;
        }

        public int TagRequirementId { get; }
        public int IntervalWeeks { get; }
        public bool IsVoided { get; }
        public string RowVersion { get; set; }
    }
}
