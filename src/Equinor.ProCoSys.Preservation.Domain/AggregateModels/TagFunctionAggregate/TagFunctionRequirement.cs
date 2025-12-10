using System;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Common.Time;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.Audit;

namespace Equinor.ProCoSys.Preservation.Domain.AggregateModels.TagFunctionAggregate
{
    public class TagFunctionRequirement : PlantEntityBase, ICreationAuditable, IModificationAuditable, IVoidable
    {
        protected TagFunctionRequirement()
            : base(null)
        {
        }

        public TagFunctionRequirement(string plant, int intervalWeeks, RequirementDefinition requirementDefinition)
            : base(plant)
        {
            if (requirementDefinition == null)
            {
                throw new ArgumentNullException(nameof(requirementDefinition));
            }

            if (requirementDefinition.Plant != plant)
            {
                throw new ArgumentException($"Can't relate item in {requirementDefinition.Plant} to item in {plant}");
            }

            IntervalWeeks = intervalWeeks;
            RequirementDefinitionId = requirementDefinition.Id;
        }

        public int IntervalWeeks { get; private set; }
        public bool IsVoided { get; set; }
        public int RequirementDefinitionId { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public int CreatedById { get; private set; }

        public DateTime? ModifiedAtUtc { get; private set; }
        public int? ModifiedById { get; private set; }

        public void SetCreated(Person createdBy)
        {
            CreatedAtUtc = TimeService.UtcNow;
            if (createdBy == null)
            {
                throw new ArgumentNullException(nameof(createdBy));
            }
            CreatedById = createdBy.Id;
        }

        public void SetModified(Person modifiedBy)
        {
            ModifiedAtUtc = TimeService.UtcNow;
            if (modifiedBy == null)
            {
                throw new ArgumentNullException(nameof(modifiedBy));
            }
            ModifiedById = modifiedBy.Id;
        }
    }
}
