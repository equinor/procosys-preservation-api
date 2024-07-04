using System;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.MessageContracts;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public interface ICreateEventHelper
{
    Task<IActionEventV1> CreateActionEvent(Action action);
    Task<ITagRequirementEventV1> CreateRequirementEvent(TagRequirement tagRequirement);
    Task<IPreservationPeriodEventV1> CreatePreservationPeriodEvent(PreservationPeriod preservationPeriod);
    Task<IFieldEventV1> CreateFieldEvent(Field field);
    Task<IRequirementDefinitionEventV1> CreateRequirementDefinitionEvent(RequirementDefinition requirementDefinition);
    Task<IRequirementTypeEventV1> CreateRequirementTypeEvent(RequirementType requirementType);
    Task<ITagEventV1> CreateTagEvent(Tag tag);
}
