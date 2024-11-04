using System;
using System.Collections.Generic;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.Audit;
using Equinor.ProCoSys.Common.Time;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.Events;

namespace Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate
{
    public class RequirementType : PlantEntityBase, IAggregateRoot, ICreationAuditable, IModificationAuditable, IVoidable, IHaveGuid
    {
        private readonly List<RequirementDefinition> _requirementDefinitions = new();

        public const int CodeLengthMax = 32;
        public const int TitleLengthMax = 64;
        public const int IconLengthMax = 32;

        protected RequirementType()
            : base(null)
        {
        }

        public RequirementType(string plant, string code, string title, RequirementTypeIcon icon, int sortKey)
            : base(plant)
        {
            Guid = Guid.NewGuid();

            Code = code;
            Title = title;
            SortKey = sortKey;
            Icon = icon;
        }

        public string Code { get; set; }
        public string Title { get; set; }
        public RequirementTypeIcon Icon { get; set; }
        public bool IsVoided { get; set; }
        public int SortKey { get; set; }
        public IReadOnlyCollection<RequirementDefinition> RequirementDefinitions => _requirementDefinitions.AsReadOnly();

        public DateTime CreatedAtUtc { get; private set; }
        public int CreatedById { get; private set; }
        public DateTime? ModifiedAtUtc { get; private set; }
        public int? ModifiedById { get; private set; }

        public Guid Guid { get; private set; }

        public void AddRequirementDefinition(RequirementDefinition requirementDefinition)
        {
            if (requirementDefinition == null)
            {
                throw new ArgumentNullException(nameof(requirementDefinition));
            }
            
            if (requirementDefinition.Plant != Plant)
            {
                throw new ArgumentException($"Can't relate item in {requirementDefinition.Plant} to item in {Plant}");
            }

            _requirementDefinitions.Add(requirementDefinition);
            
            AddDomainEvent(new ChildEntityAddedEvent<RequirementType, RequirementDefinition>(this, requirementDefinition));
        }
        
        public override string ToString() => Title;

        public void SetCreated(Person createdBy)
        {
            CreatedAtUtc = TimeService.UtcNow;
            if (createdBy == null)
            {
                throw new ArgumentNullException(nameof(createdBy));
            }
            CreatedById = createdBy.Id;

            AddDomainEvent(new PlantEntityCreatedEvent<RequirementType>(this));
        }

        public void SetModified(Person modifiedBy)
        {
            ModifiedAtUtc = TimeService.UtcNow;
            if (modifiedBy == null)
            {
                throw new ArgumentNullException(nameof(modifiedBy));
            }
            ModifiedById = modifiedBy.Id;

            AddDomainEvent(new PlantEntityModifiedEvent<RequirementType>(this));
        }

        public void RemoveRequirementDefinition(RequirementDefinition requirementDefinition)
        {
            if (requirementDefinition == null)
            {
                throw new ArgumentNullException(nameof(requirementDefinition));
            }
            
            if (!requirementDefinition.IsVoided)
            {
                throw new Exception($"{nameof(requirementDefinition)} must be voided before delete");
            }

            if (requirementDefinition.Plant != Plant)
            {
                throw new ArgumentException($"Can't remove item in {requirementDefinition.Plant} from item in {Plant}");
            }

            _requirementDefinitions.Remove(requirementDefinition);
            AddDomainEvent(new PlantEntityDeletedEvent<RequirementDefinition>(requirementDefinition));
        }
    }
}
