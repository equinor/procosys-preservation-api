using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public static class CreateRequirementTypeDeletedEventHelper
{
    public static IEnumerable<IDeleteEventV1> CreateEvents(RequirementType entity)
    {
        var typeDeletionEvent = new RequirementTypeDeleteEvent(entity.Guid, entity.Plant);
        
        var deletionEvents = GetRequirementDefinitionEvents(entity.RequirementDefinitions);
        
        return deletionEvents.Append(typeDeletionEvent);
    }
    
    private static IEnumerable<IDeleteEventV1> GetRequirementDefinitionEvents(IEnumerable<RequirementDefinition> definitions)
        => definitions.SelectMany(CreateRequirementDefinitionDeletedEventHelper.CreateEvents);
}
