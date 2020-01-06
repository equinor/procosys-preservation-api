using System;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate
{
    public class Field : SchemaEntityBase
    {
        public const int LabelLengthMax = 255;
        public const int UnitLengthMax = 32;
        
        protected Field(string schema) : base(schema)
        {
        }

        public Field(string schema, RequirementDefinition requirementDefinition, string label, string unit,
            bool isVoided, bool showPrevious, int sortKey)
            : base(schema)
        {
            if (requirementDefinition == null)
            {
                throw new ArgumentNullException(nameof(requirementDefinition));
            }

            RequirementDefId = requirementDefinition.Id;
            Label = label;
            Unit = unit;
            IsVoided = isVoided;
            ShowPrevious = showPrevious;
            SortKey = sortKey;
        }

        public int RequirementDefId { get; private set; }
        public string Label { get; private set; }
        public string Unit { get; private set; }
        public bool IsVoided { get; private set; }
        public bool ShowPrevious { get; private set; }
        public int SortKey { get; private set; }
    }
}
