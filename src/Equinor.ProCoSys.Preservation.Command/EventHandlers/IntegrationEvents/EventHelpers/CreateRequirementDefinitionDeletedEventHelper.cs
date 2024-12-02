using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public static class CreateRequirementDefinitionDeletedEventHelper
{
    public static IEnumerable<IDeleteEventV1> CreateEvents(RequirementDefinition entity)
    {
        var definitionDeletionEvent = new RequirementDefinitionDeleteEvent(entity.Guid, entity.Plant);
        
        var deletionEvents = GetFieldEvents(entity.Fields);
        
        return deletionEvents.Append(definitionDeletionEvent);
    }
    
    private static IEnumerable<IDeleteEventV1> GetFieldEvents(IEnumerable<Field> fields)
        => fields.Select(CreateFieldDeleteEventHelper.CreateEvent);
}
