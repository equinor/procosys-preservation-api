using System;
using Equinor.ProCoSys.Common;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class ActionClosedEvent : IDomainEvent
    {
        public ActionClosedEvent(string plant, Guid sourceGuid, Action action)
        {
            Plant = plant;
            SourceGuid = sourceGuid;
            Action = action;
        }
        public string Plant { get; }
        public Guid SourceGuid { get; }
        public Action Action { get; }
    }
}
