using System.Collections.Generic;
using MediatR;

namespace Equinor.Procosys.Preservation.Domain
{
    /// <summary>
    /// Base class for all entities
    /// </summary>
    public abstract class EntityBase
    {
        private List<INotification> _domainEvents;

        // Needed for EF Core
        protected EntityBase()
        {
        }

        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents?.AsReadOnly() ?? (_domainEvents = new List<INotification>()).AsReadOnly();
        public virtual int Id { get; protected set; }

        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents ??= new List<INotification>();
            _domainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(INotification eventItem)
        {
            _domainEvents?.Remove(eventItem);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}
