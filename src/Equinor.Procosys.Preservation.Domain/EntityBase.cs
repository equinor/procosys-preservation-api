using System;
using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using MediatR;

namespace Equinor.Procosys.Preservation.Domain
{
    /// <summary>
    /// Base class for all entities
    /// </summary>
    public abstract class EntityBase : IAuditable
    {
        private List<INotification> _domainEvents;

        // Needed for EF Core
        protected EntityBase()
        {
        }

        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents?.AsReadOnly() ?? (_domainEvents = new List<INotification>()).AsReadOnly();
        public virtual int Id { get; protected set; }

        public DateTime CreatedAtUtc { get; private set; }

        public int CreatedById { get; private set; }

        public DateTime? ModifiedAtUtc { get; private set; }

        public int? ModifiedById { get; private set; }

        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents ??= new List<INotification>();
            _domainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(INotification eventItem) => _domainEvents?.Remove(eventItem);

        public void ClearDomainEvents() => _domainEvents.Clear();

        public void SetCreated(DateTime createdAtUtc, Person createdBy)
        {
            if (createdAtUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException($"{nameof(createdAtUtc)} must be UTC");
            }

            CreatedAtUtc = createdAtUtc;
            CreatedById = createdBy.Id;
        }

        public void SetModified(DateTime modifiedAtUtc, Person modifiedBy)
        {
            if (modifiedAtUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException($"{nameof(modifiedAtUtc)} must be UTC");
            }

            ModifiedAtUtc = modifiedAtUtc;
            ModifiedById = modifiedBy.Id;
        }
    }
}
