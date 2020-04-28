using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MediatR;

namespace Equinor.Procosys.Preservation.Domain
{
    /// <summary>
    /// Base class for all entities
    /// </summary>
    public abstract class EntityBase
    {
        private List<INotification> _domainEvents;

        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents?.AsReadOnly() ?? (_domainEvents = new List<INotification>()).AsReadOnly();

        public virtual int Id { get; protected set; }

        public readonly byte[] rowVersion = new byte[8];
        
        public ulong RowVersion
        {
            get
            {
                return (ulong)BitConverter.ToInt64(rowVersion);
            }

            set
            { 
                var newRowVersion = BitConverter.GetBytes(value);
                for (var index = 0; index < newRowVersion.Length; index++)
                {
                    rowVersion[index] = newRowVersion[index];
                }
            }
        }

        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents ??= new List<INotification>();
            _domainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(INotification eventItem) => _domainEvents?.Remove(eventItem);

        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}
