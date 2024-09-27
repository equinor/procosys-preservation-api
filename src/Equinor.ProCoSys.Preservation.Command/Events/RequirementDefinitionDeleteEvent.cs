using System;
using Equinor.ProCoSys.Preservation.Command.Events.EntityNames;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Command.Events;

[EntityNameRequirementDefinition]
public class RequirementDefinitionDeleteEvent : DeleteEvent<RequirementDefinition>
{
    public RequirementDefinitionDeleteEvent(Guid guid, string plant) : base(guid, plant, null) {}
}
