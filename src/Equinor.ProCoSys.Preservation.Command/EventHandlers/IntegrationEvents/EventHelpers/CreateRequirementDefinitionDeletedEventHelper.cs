﻿using System.Linq;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers.EventCollections;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public static class CreateRequirementDefinitionDeletedEventHelper
{
    public static EventCollectionDeleteRequirementDefinition CreateEvents(RequirementDefinition entity)
    {
        var definitionDeletionEvent = new RequirementDefinitionDeleteEvent(entity.Guid, entity.Plant);
        var fieldsDeletedEvents = entity.Fields.Select(CreateFieldDeleteEventHelper.CreateEvent);

        return new EventCollectionDeleteRequirementDefinition(definitionDeletionEvent, fieldsDeletedEvents);
    }
}
