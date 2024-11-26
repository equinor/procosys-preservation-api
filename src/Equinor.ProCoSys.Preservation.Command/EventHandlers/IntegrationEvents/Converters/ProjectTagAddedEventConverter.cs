using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.Converters;

public class ProjectTagAddedEventConverter : IDomainToIntegrationEventConverter<ChildAddedEvent<Project, Tag>>
{
    private readonly ICreateChildEventHelper<Project, Tag, TagEvent> _createTagEventHelper;
    private readonly ICreateChildEventHelper<Project, TagRequirement, TagRequirementEvent> _createTagRequirementEventHelper;

    public ProjectTagAddedEventConverter(
        ICreateChildEventHelper<Project, Tag, TagEvent> createTagEventHelper,
        ICreateChildEventHelper<Project, TagRequirement, TagRequirementEvent> createTagRequirementEventHelper)
    {
        _createTagEventHelper = createTagEventHelper;
        _createTagRequirementEventHelper = createTagRequirementEventHelper;
    }

    public async Task<IEnumerable<IIntegrationEvent>> Convert(ChildAddedEvent<Project, Tag> domainAddedEvent)
    {
        var tagRequirementsEvents = await CreateTagRequirementEvents(domainAddedEvent);
        
        var tagEvent = await CreateTagEvent(domainAddedEvent);

        return tagRequirementsEvents.Append(tagEvent);
    }

    private async Task<IEnumerable<IIntegrationEvent>> CreateTagRequirementEvents(ChildAddedEvent<Project, Tag> domainAddedEvent)
    {
        var events = new List<IIntegrationEvent>();

        foreach (var tagRequirement in domainAddedEvent.ChildEntity.Requirements)
        {
            var tagRequirementEvent = await _createTagRequirementEventHelper.CreateEvent(domainAddedEvent.Entity, tagRequirement);

            events.Add(tagRequirementEvent);
        }

        return events;
    }
    
    private async Task<TagEvent> CreateTagEvent(ChildAddedEvent<Project, Tag> domainAddedEvent) => await _createTagEventHelper.CreateEvent(domainAddedEvent.Entity, domainAddedEvent.ChildEntity);
}
