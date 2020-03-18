using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.Audit;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.TagFunctionAggregate
{
    public class Requirement : SchemaEntityBase, ICreationAuditable, IModificationAuditable
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
            
            if (requirementDefinition.Schema != schema)
            {
                throw new ArgumentException($"Can't relate item in {requirementDefinition.Schema} to item in {schema}");
            }

            IntervalWeeks = intervalWeeks;
            RequirementDefinitionId = requirementDefinition.Id;
        }

        public int IntervalWeeks { get; private set; }
        public bool IsVoided { get; private set; }
        public int RequirementDefinitionId { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public int CreatedById { get; private set; }

        public DateTime? ModifiedAtUtc { get; private set; }
        public int? ModifiedById { get; private set; }

        public void Void() => IsVoided = true;
        public void UnVoid() => IsVoided = false;

        public void SetCreated(Person createdBy)
        {
            CreatedAtUtc = TimeService.UtcNow;
            CreatedById = createdBy.Id;
        }

        public void SetModified(Person modifiedBy)
        {
            ModifiedAtUtc = TimeService.UtcNow;
            ModifiedById = modifiedBy.Id;
        }
    }
}
