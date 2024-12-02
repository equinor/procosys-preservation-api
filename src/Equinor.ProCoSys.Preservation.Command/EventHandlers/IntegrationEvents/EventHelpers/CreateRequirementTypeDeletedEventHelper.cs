using System.Collections.Generic;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public static class CreateRequirementTypeDeletedEventHelper
{
    public static IEnumerable<IDeleteEventV1> CreateEvents(RequirementType entity)
    {
        yield return new RequirementTypeDeleteEvent(entity.Guid, entity.Plant);
        
        foreach (var requirementDefinition in entity.RequirementDefinitions)
        {
            yield return CreateRequirementDefinitionDeletedEventHelper.CreateEvent(requirementDefinition);
        }
    }
}
