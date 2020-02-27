using System;
using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.Audit;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate
{
    public class RequirementType : SchemaEntityBase, IAggregateRoot, ICreationAuditable, IModificationAuditable
    {
        private readonly List<RequirementDefinition> _requirementDefinitions = new List<RequirementDefinition>();

        public const int CodeLengthMax = 32;
        public const int TitleLengthMax = 64;

        protected RequirementType()
            : base(null)
        {
        }

        public RequirementType(string schema, string code, string title, int sortKey)
            : base(schema)
        {
            Code = code;
            Title = title;
            SortKey = sortKey;
        }

        public string Code { get; private set; }
        public string Title { get; private set; }
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

            _requirementDefinitions.Add(requirementDefinition);
        }

        public void Void() => IsVoided = true;
        public void UnVoid() => IsVoided = false;

        public void SetCreated(DateTime createdAtUtc, Person createdBy)
        {
            if (createdAtUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException($"{nameof(createdAtUtc)} is not UTC");
            }

            CreatedAtUtc = createdAtUtc;
            CreatedById = createdBy.Id;
        }

        public void SetModified(DateTime modifiedAtUtc, Person modifiedBy)
        {
            if (modifiedAtUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException($"{nameof(modifiedAtUtc)} is not UTC");
            }

            ModifiedAtUtc = modifiedAtUtc;
            ModifiedById = modifiedBy.Id;
        }
    }
}
