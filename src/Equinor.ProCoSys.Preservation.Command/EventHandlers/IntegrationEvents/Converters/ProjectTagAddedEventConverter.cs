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

    public async Task<IEnumerable<IIntegrationEvent>> Convert(ProjectTagAddedEvent domainAddedEvent)
    {
        var tagRequirementsEvents = await CreateTagRequirementEvents(domainAddedEvent);
        
        var tagEvent = await CreateTagEvent(domainAddedEvent);

        return tagRequirementsEvents.Append(tagEvent);
    }

    private async Task<IEnumerable<IIntegrationEvent>> CreateTagRequirementEvents(ProjectTagAddedEvent domainAddedEvent)
    {
        var events = new List<IIntegrationEvent>();
        
        var projectName = domainAddedEvent.Entity.Name;
        foreach (var tagRequirement in domainAddedEvent.Tag.Requirements)
        {
            var tagRequirementEvent = await _createTagRequirementEventHelper.CreateEvent(tagRequirement, projectName);

            events.Add(tagRequirementEvent);
        }

        return events;
    }
    
    private async Task<TagEvent> CreateTagEvent(ProjectTagAddedEvent domainAddedEvent)
    {
        var projectName = domainAddedEvent.Entity.Name;
        return await _createTagEventHelper.CreateEvent(domainAddedEvent.Tag, projectName);
    }
}
