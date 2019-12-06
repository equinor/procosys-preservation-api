using System.Collections.Generic;
using MediatR;

namespace Equinor.Procosys.Preservation.Domain
{
    /// <summary>
    /// Base class for all entities
    /// </summary>
    public abstract class EntityBase
    {
        private List<INotification> domainEvents;

        // Needed for EF Core
        protected EntityBase()
        {
        }

        public IReadOnlyCollection<INotification> DomainEvents => domainEvents?.AsReadOnly();
        public virtual int Id { get; protected set; }

        public void AddDomainEvent(INotification eventItem)
        {
            domainEvents = domainEvents ?? new List<INotification>();
            domainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(INotification eventItem)
        {
            domainEvents?.Remove(eventItem);
        }

        public void ClearDomainEvents()
        {
            domainEvents.Clear();
        }
    }
}
