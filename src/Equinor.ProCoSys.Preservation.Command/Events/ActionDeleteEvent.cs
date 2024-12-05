using System;
using Equinor.ProCoSys.Preservation.Command.Events.EntityNames;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Command.Events;

[EntityNameAction]
public class ActionDeleteEvent(Guid guid, string plant, string projectName)
    : DeleteEvent<Action>(guid, plant, projectName);
