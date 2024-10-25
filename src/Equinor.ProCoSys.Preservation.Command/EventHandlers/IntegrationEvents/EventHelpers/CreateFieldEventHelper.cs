using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public class CreateFieldEventHelper : ICreateEventHelper<Field, FieldEvent>
{
    private readonly IRequirementTypeRepository _requirementTypeRepository;
    private readonly ICreateChildEventHelper<RequirementDefinition, Field, FieldEvent> _createEventHelper;

    public CreateFieldEventHelper(
        IRequirementTypeRepository requirementTypeRepository,
        ICreateChildEventHelper<RequirementDefinition, Field, FieldEvent> createEventHelper)
    {
        _requirementTypeRepository = requirementTypeRepository;
        _createEventHelper = createEventHelper;
    }

    public async Task<FieldEvent> CreateEvent(Field entity)
    {
        var requirementDefinition = await _requirementTypeRepository.GetRequirementDefinitionByFieldGuidAsync(entity.Guid);

        return await _createEventHelper.CreateEvent(requirementDefinition, entity);
    }
}
