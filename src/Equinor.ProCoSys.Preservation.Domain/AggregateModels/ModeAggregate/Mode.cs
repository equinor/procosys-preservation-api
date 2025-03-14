﻿using System;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.Audit;
using Equinor.ProCoSys.Common.Time;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.Events;

namespace Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate
{
    public class Mode : PlantEntityBase, IAggregateRoot, ICreationAuditable, IModificationAuditable, IVoidable, IHaveGuid
    {
        public const int TitleLengthMin = 3;
        public const int TitleLengthMax = 255;

        protected Mode()
            : base(null)
        {
        }

        public Mode(string plant, string title, bool forSupplier) : base(plant)
        {
            Title = title;
            ForSupplier = forSupplier;
            Guid = Guid.NewGuid();
        }

        public Guid Guid { get; private set; }
        public string Title { get; set; }
        public bool IsVoided { get; set; }
        public bool ForSupplier { get; set; }
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

            AddDomainEvent(new CreatedEvent<Mode>(this));
        }

        public void SetModified(Person modifiedBy)
        {
            ModifiedAtUtc = TimeService.UtcNow;
            if (modifiedBy == null)
            {
                throw new ArgumentNullException(nameof(modifiedBy));
            }
            ModifiedById = modifiedBy.Id;

            AddDomainEvent(new ModifiedEvent<Mode>(this));
        }

        public void SetRemoved() => AddDomainEvent(new DeletedEvent<Mode>(this));
    }
}
