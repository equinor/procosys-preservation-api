using System;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class TagCreatedEvent : IPlantEntityEvent<Tag>, IDomainEvent
    {
        public TagCreatedEvent(string plant, Tag tag)
        {
            Plant = plant;
            Entity = tag;
        }
        public string Plant { get; }
        public Guid SourceGuid => Entity.Guid;
        public Tag Entity { get; }
    }
}
