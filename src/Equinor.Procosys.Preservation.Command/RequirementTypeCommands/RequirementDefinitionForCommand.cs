namespace Equinor.Procosys.Preservation.Command.RequirementTypeCommands
{
    public class RequirementDefinitionForCommand
    {
        public RequirementDefinitionForCommand(int tagRequirementId, int intervalWeeks, bool isVoided, string rowVersion)
        {
            TagRequirementId = tagRequirementId;
            IntervalWeeks = intervalWeeks;
            IsVoided = isVoided;
            RowVersion = rowVersion;
        }

        public int TagRequirementId { get;  }
        public int IntervalWeeks { get; }
        public bool IsVoided { get; }
        public string RowVersion { get; set; }
    }
}
