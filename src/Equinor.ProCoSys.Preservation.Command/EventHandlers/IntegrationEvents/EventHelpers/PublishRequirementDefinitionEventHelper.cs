using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.Audit;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public class PublishRequirementDefinitionEventHelper : IPublishEntityEventHelper<RequirementDefinition>
{
    private readonly IRequirementTypeRepository _requirementTypeRepository;
    private readonly ICreateChildEventHelper<RequirementType, RequirementDefinition, RequirementDefinitionEvent> _createEventHelper;
    private readonly IIntegrationEventPublisher _integrationEventPublisher;

    public PublishRequirementDefinitionEventHelper(
        IRequirementTypeRepository requirementTypeRepository,
        ICreateChildEventHelper<RequirementType, RequirementDefinition, RequirementDefinitionEvent> createEventHelper,
        IIntegrationEventPublisher integrationEventPublisher)
    {
        _requirementTypeRepository = requirementTypeRepository;
        _createEventHelper = createEventHelper;
        _integrationEventPublisher = integrationEventPublisher;
    }

    public async Task PublishEvent(RequirementDefinition entity, CancellationToken cancellationToken)
    {
        var requirementType = await _requirementTypeRepository.GetRequirementTypeByRequirementDefinitionGuidAsync(entity.Guid);
        
         var integrationEvent = await _createEventHelper.CreateEvent(requirementType, entity);
         await _integrationEventPublisher.PublishAsync(integrationEvent, cancellationToken);
    }
}
