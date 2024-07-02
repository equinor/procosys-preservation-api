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
    Task<IActionEventV1> CreateActionEvent(Action action, Guid tagGuid);
    Task SendTagRequirementEvents(TagRequirement tagRequirement, Guid tagGuid, CancellationToken cancellationToken);
    Task<ITagRequirementEventV1> CreateRequirementEvent(TagRequirement tagRequirement, Guid tagGuid);
    Task<IPreservationPeriodEventV1> CreatePreservationPeriodEvent(PreservationPeriod preservationPeriod, Guid tagRequirementGuid, Guid tagGuid);
    Task<IPreservationRecordEventV1> CreatePreservationRecordEvent(PreservationRecord preservationRecord, Guid tagRequirementGuid, Guid preservationPeriodGuid, Guid tagGuid);
    Task<IFieldEventV1> CreateFieldEvent(Field field, Guid sourceGuid);
    Task<IRequirementDefinitionEventV1> CreateRequirementDefinitionEvent(RequirementDefinition requirementDefinition);
}
