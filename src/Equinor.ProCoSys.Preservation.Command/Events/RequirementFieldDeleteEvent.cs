using System;
using Equinor.ProCoSys.Preservation.Command.Events.EntityNames;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Command.Events;

[RequirementFieldEntityName]
public class RequirementFieldDeleteEvent : DeleteEvent<Field>
{
    public RequirementFieldDeleteEvent(Guid guid, string plant) : base(guid, plant, null) {}
}
