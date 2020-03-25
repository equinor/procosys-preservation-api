using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.Audit;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.TagFunctionAggregate
{
    public class TagFunction : SchemaEntityBase, IAggregateRoot, ICreationAuditable, IModificationAuditable
    {
        public const int CodeLengthMax = 255;
        public const int DescriptionLengthMax = 255;
        public const int RegisterCodeLengthMax = 255;

        private readonly List<TagFunctionRequirement> _requirements = new List<TagFunctionRequirement>();

        protected TagFunction()
            : base(null)
        {
        }

        public TagFunction(string schema, string code, string description, string registerCode)
            : base(schema)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            if (string.IsNullOrEmpty(registerCode))
            {
                throw new ArgumentNullException(nameof(registerCode));
            }

            Code = code;
            Description = description;
            RegisterCode = registerCode;
        }

        public IReadOnlyCollection<TagFunctionRequirement> Requirements => _requirements.AsReadOnly();
        public string Code { get; private set; }
        public string Description { get; private set; }
        public string RegisterCode { get; private set; }
        public bool IsVoided { get; private set; }

        public DateTime CreatedAtUtc { get; private set; }
        public int CreatedById { get; private set; }
        public DateTime? ModifiedAtUtc { get; private set; }
        public int? ModifiedById { get; private set; }

        public void Void() => IsVoided = true;
        public void UnVoid() => IsVoided = false;

        public void AddRequirement(TagFunctionRequirement requirement)
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }
            
            if (requirement.Schema != Schema)
            {
                throw new ArgumentException($"Can't relate item in {requirement.Schema} to item in {Schema}");
            }

            if (_requirements.Any(r => r.RequirementDefinitionId == requirement.RequirementDefinitionId))
            {
                throw new ArgumentException($"{nameof(TagFunction)} {Code} in register {RegisterCode} already have requirement with definition {requirement.RequirementDefinitionId}");
            }

            _requirements.Add(requirement);
        }

        public void RemoveRequirement(TagFunctionRequirement requirement)
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }
            
            if (requirement.Schema != Schema)
            {
                throw new ArgumentException($"Can't remove item in {requirement.Schema} from item in {Schema}");
            }

            _requirements.Remove(requirement);
        }

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
