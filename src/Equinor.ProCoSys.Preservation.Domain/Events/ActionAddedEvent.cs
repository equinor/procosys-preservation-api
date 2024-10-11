using System;
using Equinor.ProCoSys.Common;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class ActionAddedEvent : IPlantEntityEvent<Action>, IDomainEvent
    {
        public ActionAddedEvent(string plant, Guid sourceGuid, Action action)
        {
            Plant = plant;
            SourceGuid = sourceGuid;
            Entity = action;
        }
        public string Plant { get; }
        public Guid SourceGuid { get; }
        public Action Entity { get; }
    }
}
