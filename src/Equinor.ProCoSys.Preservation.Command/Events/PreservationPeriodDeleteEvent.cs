using System;
using Equinor.ProCoSys.Preservation.Command.Events.EntityNames;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Command.Events;

[PreservationPeriodEntityName]
public class PreservationPeriodDeleteEvent : DeleteEvent<PreservationPeriod>
{
    public PreservationPeriodDeleteEvent(Guid guid, string plant) : base(guid, plant, null) {}
}
