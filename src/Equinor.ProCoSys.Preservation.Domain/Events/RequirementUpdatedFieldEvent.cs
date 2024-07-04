using System;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class RequirementUpdatedFieldEvent : IDomainEvent
{
    public RequirementUpdatedFieldEvent(Field field) => Field = field;
    public Field Field { get; }
}
