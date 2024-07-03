using System;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class RequirementAddedFieldEvent : IDomainEvent
{
    public RequirementAddedFieldEvent(Guid requirementDefinitionGuid, Field field)
    {
        this.RequirementDefinitionGuid = requirementDefinitionGuid;
        this.Field = field;
    }

    public Guid RequirementDefinitionGuid { get; }
    public Field Field { get; }
}
