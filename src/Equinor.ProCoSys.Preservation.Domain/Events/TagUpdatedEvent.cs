using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class TagUpdatedEvent : IPlantEntityEvent<Tag>, IDomainEvent
    {
        public TagUpdatedEvent(Tag tag) => Entity = tag;
        public Tag Entity { get; }
    }
}
