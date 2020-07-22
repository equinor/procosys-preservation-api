using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.Procosys.Preservation.Command.RequirementTypeCommands.UpdateRequirementDefinition
{
    public class UpdateFieldsForCommand
    {
        public UpdateFieldsForCommand(
            int id,
            string label,
            FieldType fieldType,
            int sortKey,
            string rowVersion,
            string unit = null,
            bool? delete = false,
            bool? showPrevious = null)
        {
            Id = id;
            Label = label;
            SortKey = sortKey;
            FieldType = fieldType;
            ShowPrevious = showPrevious;
            Unit = unit;
            Delete = delete;
            RowVersion = rowVersion;
        }

        public int Id { get; }
        public string Label { get; }
        public string Unit { get; private set; }
        public bool? ShowPrevious { get; }
        public int SortKey { get; }
        public FieldType FieldType { get; }
        public string RowVersion { get;  }
        public bool? Delete { get; }
    }
}
