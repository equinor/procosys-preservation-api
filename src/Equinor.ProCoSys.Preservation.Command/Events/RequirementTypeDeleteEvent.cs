using System;
using Equinor.ProCoSys.Preservation.Command.Events.EntityNames;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Command.Events;

[EntityNameRequirementType]
public class RequirementTypeDeleteEvent : DeleteEvent<RequirementType>
{
    public RequirementTypeDeleteEvent(Guid guid, string plant) : base(guid, plant, null) {}
}
