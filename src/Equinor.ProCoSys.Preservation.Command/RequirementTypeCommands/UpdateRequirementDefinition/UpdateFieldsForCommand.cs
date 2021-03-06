﻿using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Command.RequirementTypeCommands.UpdateRequirementDefinition
{
    public class UpdateFieldsForCommand
    {
        public UpdateFieldsForCommand(
            int id,
            string label,
            FieldType fieldType,
            int sortKey,
            bool isVoided,
            string rowVersion,
            string unit = null,
            bool? showPrevious = null)
        {
            Id = id;
            Label = label;
            SortKey = sortKey;
            IsVoided = isVoided;
            FieldType = fieldType;
            ShowPrevious = showPrevious;
            Unit = unit;
            RowVersion = rowVersion;
        }

        public int Id { get; }
        public string Label { get; }
        public string Unit { get; private set; }
        public bool? ShowPrevious { get; }
        public bool IsVoided { get; }
        public int SortKey { get; }
        public FieldType FieldType { get; }
        public string RowVersion { get;  }
    }
}
