using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate
{
    public class Requirement : SchemaEntityBase
    {
        protected Requirement()
            : base(null)
        {
        }

        public Requirement(string schema, int intervalWeeks, RequirementDefinition requirementDefinition)
            : base(schema)
        {
            if (requirementDefinition == null)
            {
                throw new ArgumentNullException(nameof(requirementDefinition));
            }

            IntervalWeeks = intervalWeeks;
            RequirementDefinitionId = requirementDefinition.Id;
        }
        public int IntervalWeeks { get; private set; }
        public bool IsVoided { get; private set; }
        public int RequirementDefinitionId { get; set; }

        public void Void() => IsVoided = true;
        public void UnVoid() => IsVoided = false;
    }
}
