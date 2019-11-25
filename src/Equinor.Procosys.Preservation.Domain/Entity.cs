using System.Collections.Generic;
using MediatR;

namespace Equinor.Procosys.Preservation.Domain
{
    public abstract class Entity
    {
        private List<INotification> domainEvents;

        public IReadOnlyCollection<INotification> DomainEvents => domainEvents?.AsReadOnly();
        public int Id { get; protected set; }

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
