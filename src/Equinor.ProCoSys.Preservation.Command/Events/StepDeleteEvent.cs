using System;
using Equinor.ProCoSys.Preservation.Command.Events.EntityNames;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;

namespace Equinor.ProCoSys.Preservation.Command.Events;

[EntityNameStep]
public class StepDeleteEvent : DeleteEvent<Step>
{
    public StepDeleteEvent(Guid guid, string plant) : base(guid, plant, null) {}
}
