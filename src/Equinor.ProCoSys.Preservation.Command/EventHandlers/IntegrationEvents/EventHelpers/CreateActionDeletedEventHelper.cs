﻿using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public static class CreateActionDeletedEventHelper
{
    public static ActionDeleteEvent CreateEvent(Action entity, Project project) => new(entity.Guid, entity.Plant, project.Name);
}
