using System;
using Equinor.ProCoSys.Preservation.Command.Events.EntityNames;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Command.Events;

[EntityNameField]
public class FieldDeleteEvent : DeleteEvent<Field>
{
    public FieldDeleteEvent(Guid guid, string plant) : base(guid, plant, null) {}
}
