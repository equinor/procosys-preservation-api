using System;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class RequirementUpdatedFieldEvent : IDomainEvent
{
    public RequirementUpdatedFieldEvent(Guid requirementDefinitionGuid, Field field)
    {
        RequirementDefinitionGuid = requirementDefinitionGuid;
        Field = field;
    }

    public Guid RequirementDefinitionGuid { get; }
    public Field Field { get; }
}
