using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.HistoryAggregate;
using MediatR;

namespace Equinor.Procosys.Preservation.Domain.Events
{
    public class HistoryEvent : INotification
    {
        public HistoryEvent(
            Guid objectGuid,
            string plant,
            EventType eventType,
            ObjectType objectType)
        {
            ObjectGuid = objectGuid;
            Plant = plant;
            EventType = eventType;
            ObjectType = objectType;
        }

        public Guid ObjectGuid { get; }
        public string Plant { get; }
        public EventType EventType { get; }
        public ObjectType ObjectType { get; }
    }
}
