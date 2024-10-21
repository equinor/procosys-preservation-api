using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.Converters;

public class ProjectTagAddedEventConverter : IDomainToIntegrationEventConverter<ProjectTagAddedEvent>
{
    private readonly ICreateProjectEventHelper<TagRequirement, TagRequirementEvent> _createTagRequirementEvent;

    public ProjectTagAddedEventConverter(ICreateProjectEventHelper<TagRequirement, TagRequirementEvent> createTagRequirementEvent) => _createTagRequirementEvent = createTagRequirementEvent;

    public async Task<IEnumerable<IIntegrationEvent>> Convert(ProjectTagAddedEvent domainEvent)
    {
        var events = new List<IIntegrationEvent>();

        foreach (var tagRequirement in domainEvent.Tag.Requirements)
        {
            var projectName = domainEvent.Entity.Name;
            var tagRequirementEvent = await _createTagRequirementEvent.CreateEvent(tagRequirement, projectName);

            events.Add(tagRequirementEvent);
        }

        return events;
    }
}
