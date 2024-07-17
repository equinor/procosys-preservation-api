using System;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class RequirementAddedFieldEvent : IPostSaveDomainEvent
{
    public RequirementAddedFieldEvent(Field field) => this.Field = field;
    public Field Field { get; }
}
