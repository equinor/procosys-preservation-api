using System;
using Equinor.ProCoSys.Common;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class ActionUpdatedEvent : IDomainEvent
    {
        public ActionUpdatedEvent(string plant, Action action)
        {
            Plant = plant;
            Action = action;
        }
        public string Plant { get; }
        public Action Action { get; }
    }
}
