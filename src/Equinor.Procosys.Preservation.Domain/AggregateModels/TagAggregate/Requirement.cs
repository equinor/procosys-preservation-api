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
        public Requirement(string schema, int interval, RequirementDefinition requirementDefinition)
            : base(schema)
        {
            if (requirementDefinition == null)
            {
                throw new ArgumentNullException(nameof(requirementDefinition));
            }

            Interval = interval;
            RequirementDefinitionId = requirementDefinition.Id;
        }
        public int Interval { get; private set; }
        public bool IsVoided { get; private set; }
        public int RequirementDefinitionId { get; set; }

        public void Void() => IsVoided = true;
        public void UnVoid() => IsVoided = false;
    }
}
