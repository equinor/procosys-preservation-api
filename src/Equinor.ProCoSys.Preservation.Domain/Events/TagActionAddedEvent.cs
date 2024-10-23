using System;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class TagActionAddedEvent : IPlantEntityEvent<Tag>, IDomainEvent
    {
        public TagActionAddedEvent(Tag tag, Action action)
        {
            Entity = tag;
            Action = action;
        }
        public Tag Entity { get; }
        public Action Action { get; }
    }
}
