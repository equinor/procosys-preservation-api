using System;
using System.Collections.Generic;
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

        public DateTime Created { get; private set; }

        public int CreatedById { get; private set; }

        public DateTime? Modified { get; private set; }

        public int? ModifiedById { get; private set; }

        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents ??= new List<INotification>();
            _domainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(INotification eventItem) => _domainEvents?.Remove(eventItem);

        public void ClearDomainEvents() => _domainEvents.Clear();

        public void SetCreated(DateTime creationDate, int createdById)
        {
            Created = creationDate;
            CreatedById = createdById;
        }

        public void SetModified(DateTime modifiedDate, int modifiedById)
        {
            Modified = modifiedDate;
            ModifiedById = modifiedById;
        }
    }
}
