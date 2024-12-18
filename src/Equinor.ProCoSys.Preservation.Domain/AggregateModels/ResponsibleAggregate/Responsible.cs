﻿using System;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.Audit;
using Equinor.ProCoSys.Common.Time;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.Events;

namespace Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate
{
    public class Responsible : PlantEntityBase, IAggregateRoot, ICreationAuditable, IModificationAuditable, IVoidable, IHaveGuid
    {
        public const int CodeLengthMax = 255;
        public const int DescriptionLengthMax = 255;

        protected Responsible()
            : base(null)
        {
        }

        public Responsible(string plant, string code, string description)
            : base(plant)
        {
            Guid = Guid.NewGuid();
            Code = code;
            Description = description;
        }

        public Guid Guid { get; private set; }

        public string Code { get; private set; }
        public string Description { get; set; }
        public bool IsVoided { get; set; }

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

            AddDomainEvent(new CreatedEvent<Responsible>(this));
        }

        public void SetModified(Person modifiedBy)
        {
            ModifiedAtUtc = TimeService.UtcNow;
            if (modifiedBy == null)
            {
                throw new ArgumentNullException(nameof(modifiedBy));
            }
            ModifiedById = modifiedBy.Id;

            AddDomainEvent(new ModifiedEvent<Responsible>(this));
        }

        public void RenameResponsible(string newCode)
        {
            if (string.IsNullOrWhiteSpace(newCode))
            {
                throw new ArgumentNullException(nameof(newCode));
            }

            Code = newCode;
        }

        public void SetRemoved() => AddDomainEvent(new DeletedEvent<Responsible>(this));
    }
}
