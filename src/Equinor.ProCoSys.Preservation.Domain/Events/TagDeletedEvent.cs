using System;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class TagDeletedEvent : IPlantEntityEvent<Tag>, IDomainEvent
    {
        public TagDeletedEvent(Tag tag) => Entity = tag;
        public Tag Entity { get; }
    }
}
