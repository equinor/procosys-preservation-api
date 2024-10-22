using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.Converters;

public class RequirementTypeRequirementDefinitionAddedEventConverter : IDomainToIntegrationEventConverter<RequirementTypeRequirementDefinitionAddedEvent>
{
    private readonly ICreateChildEventHelper<RequirementType, RequirementDefinition, RequirementDefinitionEvent> _createEventHelper;

    public RequirementTypeRequirementDefinitionAddedEventConverter(ICreateChildEventHelper<RequirementType, RequirementDefinition, RequirementDefinitionEvent> createEventHelper) => _createEventHelper = createEventHelper;

    public async Task<IEnumerable<IIntegrationEvent>> Convert(RequirementTypeRequirementDefinitionAddedEvent domainAddedEvent)
    {
        var requirementDefinitionEvent = await _createEventHelper.CreateEvent(domainAddedEvent.Entity, domainAddedEvent.RequirementDefinition);

        return new List<IIntegrationEvent> { requirementDefinitionEvent };
    }
}
