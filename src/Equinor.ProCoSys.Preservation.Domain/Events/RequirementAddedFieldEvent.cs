using System;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class RequirementAddedFieldEvent : IPlantEntityEvent<Field>, IPostSaveDomainEvent
{
    public RequirementAddedFieldEvent(Field field) => Entity = field;
    public Field Entity { get; }
}
