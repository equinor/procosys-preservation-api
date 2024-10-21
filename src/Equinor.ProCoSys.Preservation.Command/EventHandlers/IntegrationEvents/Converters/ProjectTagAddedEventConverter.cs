using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.Converters;

public class ProjectTagAddedEventConverter : IDomainToIntegrationEventConverter<ProjectTagAddedEvent>
{
    private readonly ICreateProjectEventHelper<Tag, TagEvent> _createTagEvent;
    private readonly ICreateProjectEventHelper<TagRequirement, TagRequirementEvent> _createTagRequirementEvent;

    public ProjectTagAddedEventConverter(
        ICreateProjectEventHelper<Tag, TagEvent> createTagEvent,
        ICreateProjectEventHelper<TagRequirement, TagRequirementEvent> createTagRequirementEvent)
    {
        _createTagEvent = createTagEvent;
        _createTagRequirementEvent = createTagRequirementEvent;
    }

    public async Task<IEnumerable<IIntegrationEvent>> Convert(ProjectTagAddedEvent domainEvent)
    {
        var tagRequirementsEvents = await ParseTagRequirementEvents(domainEvent);
        
        var tagEvent = await ParseTagEvent(domainEvent);

        return tagRequirementsEvents.Append(tagEvent);
    }

    private async Task<IEnumerable<IIntegrationEvent>> ParseTagRequirementEvents(ProjectTagAddedEvent domainEvent)
    {
        var events = new List<IIntegrationEvent>();
        
        var projectName = domainEvent.Entity.Name;
        foreach (var tagRequirement in domainEvent.Tag.Requirements)
        {
            var tagRequirementEvent = await _createTagRequirementEvent.CreateEvent(tagRequirement, projectName);

            events.Add(tagRequirementEvent);
        }

        return events;
    }
    
    private async Task<TagEvent> ParseTagEvent(ProjectTagAddedEvent domainEvent)
    {
        var projectName = domainEvent.Entity.Name;
        return await _createTagEvent.CreateEvent(domainEvent.Tag, projectName);
    }
}
