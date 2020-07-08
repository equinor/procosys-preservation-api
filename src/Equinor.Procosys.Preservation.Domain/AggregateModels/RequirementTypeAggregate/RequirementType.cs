using System;
using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.Audit;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate
{
    public class RequirementType : PlantEntityBase, IAggregateRoot, ICreationAuditable, IModificationAuditable
    {
        private readonly List<RequirementDefinition> _requirementDefinitions = new List<RequirementDefinition>();

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
            Code = code;
            Title = title;
            SortKey = sortKey;
            Icon = icon;
        }

        public string Code { get; private set; }
        public string Title { get; private set; }
        public RequirementTypeIcon Icon { get; private set; }
        public bool IsVoided { get; private set; }
        public int SortKey { get; private set; }
        public IReadOnlyCollection<RequirementDefinition> RequirementDefinitions => _requirementDefinitions.AsReadOnly();

        public DateTime CreatedAtUtc { get; private set; }
        public int CreatedById { get; private set; }
        public DateTime? ModifiedAtUtc { get; private set; }
        public int? ModifiedById { get; private set; }

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
        }

        public void Void() => IsVoided = true;
        public void UnVoid() => IsVoided = false;
        
        public override string ToString() => Title;

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
