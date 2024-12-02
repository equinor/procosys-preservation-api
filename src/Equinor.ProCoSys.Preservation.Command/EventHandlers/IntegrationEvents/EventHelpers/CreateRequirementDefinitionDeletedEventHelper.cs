using System.Collections.Generic;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public static class CreateRequirementDefinitionDeletedEventHelper
{
    public static IEnumerable<IDeleteEventV1> CreateEvents(RequirementDefinition entity)
    {
        yield return new RequirementDefinitionDeleteEvent(entity.Guid, entity.Plant);
    }
}
