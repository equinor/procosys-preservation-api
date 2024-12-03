using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public static class CreateRequirementTypeDeletedEventHelper
{
    public static RequirementTypeDeletedEvents CreateEvents(RequirementType entity)
    {
        var typeDeletionEvent = new RequirementTypeDeleteEvent(entity.Guid, entity.Plant);
        
        var deletionEvents = entity.RequirementDefinitions.Select(CreateRequirementDefinitionDeletedEventHelper.CreateEvents).ToList();
        var definitionDeletedEvents = deletionEvents.Select(e => e.DefinitionDeleteEvent);
        var fieldDeletedEvents = deletionEvents.SelectMany(e => e.FieldDeleteEvents);
        
        return new RequirementTypeDeletedEvents(typeDeletionEvent, definitionDeletedEvents, fieldDeletedEvents);
    }
}
