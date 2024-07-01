using System;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class ActionAddedEvent : IDomainEvent
    {
        public ActionAddedEvent(string plant, Guid sourceGuid, Action action, string title)
        {
            Plant = plant;
            SourceGuid = sourceGuid;
            Title = title;
            Action = action;
        }
        public string Plant { get; }
        public Guid SourceGuid { get; }
        public string Title { get; }
        public Action Action { get; }
    }
}
