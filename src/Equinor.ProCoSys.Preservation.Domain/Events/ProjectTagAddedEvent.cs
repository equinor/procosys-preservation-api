using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class ProjectTagAddedEvent : IPlantEntityEvent<Project>, IDomainEvent
    {
        public ProjectTagAddedEvent(Project project, Tag tag)
        {
            Entity = project;
            Tag = tag;
        }

        public Project Entity { get; }
        public Tag Tag { get; }
    }
}
