using System.Linq;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers.EventCollections;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public static class CreateRequirementTypeDeletedEventHelper
{
    public static EventCollectionDeleteRequirementType CreateEvents(RequirementType entity)
    {
        var typeDeletionEvent = new RequirementTypeDeleteEvent(entity.Guid, entity.Plant);

        var deletionEvents = entity.RequirementDefinitions.Select(CreateRequirementDefinitionDeletedEventHelper.CreateEvents).ToList();
        var definitionDeletedEvents = deletionEvents.Select(e => e.DefinitionDeleteEvent);
        var fieldDeletedEvents = deletionEvents.SelectMany(e => e.FieldDeleteEvents);

        return new EventCollectionDeleteRequirementType(typeDeletionEvent, definitionDeletedEvents, fieldDeletedEvents);
    }
}
