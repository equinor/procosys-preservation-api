﻿using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Command.RequirementTypeCommands
{
    public class FieldsForCommand
    {
        public FieldsForCommand(
            string label,
            FieldType fieldType,
            int sortKey,
            string unit = null,
            bool? showPrevious = null)
        {
            Label = label;
            SortKey = sortKey;
            FieldType = fieldType;
            ShowPrevious = showPrevious;
            Unit = unit;
        }

        public string Label { get; }
        public string Unit { get; private set; }
        public bool? ShowPrevious { get; }
        public int SortKey { get; }
        public FieldType FieldType { get; }
    }
}
