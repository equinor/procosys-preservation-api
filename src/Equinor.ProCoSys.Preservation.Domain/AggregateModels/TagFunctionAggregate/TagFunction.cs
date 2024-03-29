﻿using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.Audit;
using Equinor.ProCoSys.Common.Time;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.AggregateModels.TagFunctionAggregate
{
    public class TagFunction : PlantEntityBase, IAggregateRoot, ICreationAuditable, IModificationAuditable, IVoidable
    {
        public const int CodeLengthMax = 255;
        public const int DescriptionLengthMax = 255;
        public const int RegisterCodeLengthMax = 255;

        private readonly List<TagFunctionRequirement> _requirements = new List<TagFunctionRequirement>();

        protected TagFunction()
            : base(null)
        {
        }

        public TagFunction(string plant, string code, string description, string registerCode)
            : base(plant)
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
        public string Description { get; set; }
        public string RegisterCode { get; private set; }
        public bool IsVoided { get; set; }

        public DateTime CreatedAtUtc { get; private set; }
        public int CreatedById { get; private set; }
        public DateTime? ModifiedAtUtc { get; private set; }
        public int? ModifiedById { get; private set; }

        public void AddRequirement(TagFunctionRequirement requirement)
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }
            
            if (requirement.Plant != Plant)
            {
                throw new ArgumentException($"Can't relate item in {requirement.Plant} to item in {Plant}");
            }

            if (_requirements.Any(r => r.RequirementDefinitionId == requirement.RequirementDefinitionId))
            {
                throw new ArgumentException($"{nameof(TagFunction)} {Code} in register {RegisterCode} already has a requirement with definition {requirement.RequirementDefinitionId}");
            }

            _requirements.Add(requirement);
        }

        public void RemoveRequirement(TagFunctionRequirement requirement)
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }
            
            if (requirement.Plant != Plant)
            {
                throw new ArgumentException($"Can't remove item in {requirement.Plant} from item in {Plant}");
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
        public void RenameTagFunction(string newCode, string newRegisterCode)
        {
            if (string.IsNullOrWhiteSpace(newCode) || string.IsNullOrWhiteSpace(newRegisterCode))
            {
                throw new ArgumentNullException($"{nameof(newCode)} or {nameof(newRegisterCode)}");
            }
            Code = newCode;
            RegisterCode = newRegisterCode;
        }
    }
}
