using System;
using Equinor.ProCoSys.Common;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class ActionUpdatedEvent : IDomainEvent
    {
        public ActionUpdatedEvent(string plant, Guid tagGuid, Action action)
        {
            Plant = plant;
            TagGuid = tagGuid;
            Action = action;
        }
        public string Plant { get; }
        public Guid TagGuid { get; }
        public Action Action { get; }
    }
}
