using System;
using Equinor.ProCoSys.Preservation.Command.Events.EntityNames;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;

namespace Equinor.ProCoSys.Preservation.Command.Events;

[EntityNameMode]
public class ModeDeleteEvent : DeleteEvent<Mode>
{
    public ModeDeleteEvent(Guid guid, string plant) : base(guid, plant, null) {}
}
