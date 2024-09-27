using System;
using Equinor.ProCoSys.Preservation.Command.Events.EntityNames;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;

namespace Equinor.ProCoSys.Preservation.Command.Events;

[JourneyEntityName]
public class JourneyDeleteEvent : DeleteEvent<Journey>
{
    public JourneyDeleteEvent(Guid guid, string plant) : base(guid, plant, null) {}
}
