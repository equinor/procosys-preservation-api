using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public class CreateRequirementDefinitionEventHelper : ICreateEventHelper<RequirementDefinition, RequirementDefinitionEvent>
{
    private readonly IRequirementTypeRepository _requirementTypeRepository;
    private readonly ICreateChildEventHelper<RequirementType, RequirementDefinition, RequirementDefinitionEvent> _createEventHelper;

    public CreateRequirementDefinitionEventHelper(
        IRequirementTypeRepository requirementTypeRepository,
        ICreateChildEventHelper<RequirementType, RequirementDefinition, RequirementDefinitionEvent> createEventHelper)
    {
        _requirementTypeRepository = requirementTypeRepository;
        _createEventHelper = createEventHelper;
    }

    public async Task<RequirementDefinitionEvent> CreateEvent(RequirementDefinition entity)
    {
        var requirementType = await _requirementTypeRepository.GetRequirementTypeByRequirementDefinitionGuidAsync(entity.Guid);
        return await _createEventHelper.CreateEvent(requirementType, entity);
    }
}
