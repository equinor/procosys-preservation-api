using System;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class ActionAddedEvent : IPlantEntityEvent<Action>, IDomainEvent
    {
        public ActionAddedEvent(string plant, Action action)
        {
            Plant = plant;
            Entity = action;
        }
        public string Plant { get; }
        public Action Entity { get; }
    }
}
