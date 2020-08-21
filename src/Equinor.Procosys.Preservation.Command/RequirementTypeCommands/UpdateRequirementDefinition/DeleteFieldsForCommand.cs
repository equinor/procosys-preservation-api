namespace Equinor.Procosys.Preservation.Command.RequirementTypeCommands.UpdateRequirementDefinition
{
    public class DeleteFieldsForCommand
    {
        public DeleteFieldsForCommand(
            int id,
            string rowVersion)
        {
            Id = id;
            RowVersion = rowVersion;
        }

        public int Id { get; }
        public string RowVersion { get;  }
    }
}
