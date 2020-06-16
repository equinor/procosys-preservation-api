using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.HistoryAggregate;
using MediatR;

namespace Equinor.Procosys.Preservation.Domain.Events
{
    public class HistoryEvent : INotification
    {
        public HistoryEvent(
            string plant,
            Guid objectGuid,
            EventType eventType,
            ObjectType objectType)
        {
            Plant = plant;
            ObjectGuid = objectGuid;
            EventType = eventType;
            ObjectType = objectType;
        }
        public string Plant { get; }
        public Guid ObjectGuid { get; }
        public EventType EventType { get; }
        public ObjectType ObjectType { get; }
    }
}
