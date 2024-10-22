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
    private readonly ICreateProjectEventHelper<Tag, TagEvent> _createTagEventHelper;
    private readonly ICreateProjectEventHelper<TagRequirement, TagRequirementEvent> _createTagRequirementEventHelper;

    public ProjectTagAddedEventConverter(
        ICreateProjectEventHelper<Tag, TagEvent> createTagEventHelper,
        ICreateProjectEventHelper<TagRequirement, TagRequirementEvent> createTagRequirementEventHelper)
    {
        _createTagEventHelper = createTagEventHelper;
        _createTagRequirementEventHelper = createTagRequirementEventHelper;
    }

    public async Task<IEnumerable<IIntegrationEvent>> Convert(ProjectTagAddedEvent domainEvent)
    {
        var tagRequirementsEvents = await CreateTagRequirementEvents(domainEvent);
        
        var tagEvent = await CreateTagEvent(domainEvent);

        return tagRequirementsEvents.Append(tagEvent);
    }

    private async Task<IEnumerable<IIntegrationEvent>> CreateTagRequirementEvents(ProjectTagAddedEvent domainEvent)
    {
        var events = new List<IIntegrationEvent>();
        
        var projectName = domainEvent.Entity.Name;
        foreach (var tagRequirement in domainEvent.Tag.Requirements)
        {
            var tagRequirementEvent = await _createTagRequirementEventHelper.CreateEvent(tagRequirement, projectName);

            events.Add(tagRequirementEvent);
        }

        return events;
    }
    
    private async Task<TagEvent> CreateTagEvent(ProjectTagAddedEvent domainEvent)
    {
        var projectName = domainEvent.Entity.Name;
        return await _createTagEventHelper.CreateEvent(domainEvent.Tag, projectName);
    }
}
