using Equinor.ProCoSys.Common;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class ActionClosedEvent : IPlantEntityEvent<Action>, IDomainEvent
    {
        public ActionClosedEvent(string plant, Action action)
        {
            Plant = plant;
            Entity = action;
        }
        public string Plant { get; }
        public Action Entity { get; }
    }
}
