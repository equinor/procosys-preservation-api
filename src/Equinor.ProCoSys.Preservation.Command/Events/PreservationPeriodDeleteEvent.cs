using System;
using Equinor.ProCoSys.Preservation.Command.Events.EntityNames;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Command.Events;

[EntityNameTagRequirement]
public class PreservationPeriodDeleteEvent(Guid guid, string plant, string projectName)
    : DeleteEvent<PreservationPeriod>(guid, plant, projectName);
