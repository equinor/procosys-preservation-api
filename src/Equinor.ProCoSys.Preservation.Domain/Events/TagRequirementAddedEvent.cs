using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class TagRequirementAddedEvent : IPlantEntityEvent<TagRequirement>, IDomainEvent
    {
        public TagRequirementAddedEvent(string plant, TagRequirement tagRequirement)
        {
            Plant = plant;
            Entity = tagRequirement;
        }

        public string Plant { get; }
        public TagRequirement Entity { get; }
    }
}
