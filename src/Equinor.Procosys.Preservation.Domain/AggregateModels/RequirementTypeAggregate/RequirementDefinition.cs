using System;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate
{
    public class RequirementDefinition : SchemaEntityBase
    {
        public const int TitleLengthMax = 64;

        protected RequirementDefinition()
            : base(null)
        {
        }

        public RequirementDefinition(string schema, RequirementType requirementType, string title, bool isVoided, int defaultInterval, int sortKey)
            : base(schema)
        {
            if (requirementType == null)
            {
                throw new ArgumentNullException(nameof(requirementType));
            }
            RequirementTypeId = requirementType.Id;
            Title = title;
            IsVoided = isVoided;
            DefaultInterval = defaultInterval;
            SortKey = sortKey;
        }

        public int RequirementTypeId { get; private set; }
        public string Title { get; private set; }
        public bool IsVoided { get; private set; }
        public int DefaultInterval { get; private set; }
        public int SortKey { get; private set; }
    }
}
