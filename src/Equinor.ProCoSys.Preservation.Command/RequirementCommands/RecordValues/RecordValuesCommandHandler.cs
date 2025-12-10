using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.RequirementCommands.RecordValues
{
    public class RecordValuesCommandHandler : IRequestHandler<RecordValuesCommand, Result<Unit>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IRequirementTypeRepository _requirementTypeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RecordValuesCommandHandler(
            IProjectRepository projectRepository,
            IRequirementTypeRepository requirementTypeRepository,
            IUnitOfWork unitOfWork)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
            _requirementTypeRepository = requirementTypeRepository;
        }

        public async Task<Result<Unit>> Handle(RecordValuesCommand request, CancellationToken cancellationToken)
        {
            var tag = await _projectRepository.GetTagWithPreservationHistoryByTagIdAsync(request.TagId);
            var requirement = tag.Requirements.Single(r => r.Id == request.RequirementId);

            var requirementDefinition =
                await _requirementTypeRepository.GetRequirementDefinitionByIdAsync(requirement.RequirementDefinitionId);

            RecordCheckBoxValues(request.CheckBoxValues, requirement, requirementDefinition);

            RecordNumberIsNaValues(request.NumberValues, requirement, requirementDefinition);

            RecordNumberValues(request.NumberValues, requirement, requirementDefinition);

            requirement.SetComment(request.Comment);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new SuccessResult<Unit>(Unit.Value);
        }

        private static void RecordNumberValues(List<NumberFieldValue> values, TagRequirement requirement, RequirementDefinition requirementDefinition)
        {
            var numberValues =
                values.Where(nv => !nv.IsNa).ToDictionary(
                    keySelector => keySelector.FieldId,
                    elementSelector => elementSelector.Value);
            requirement.RecordNumberValues(numberValues, requirementDefinition);
        }

        private static void RecordNumberIsNaValues(List<NumberFieldValue> values, TagRequirement requirement, RequirementDefinition requirementDefinition)
        {
            IList<int> fieldIds = values.Where(nv => nv.IsNa).Select(nv => nv.FieldId).ToList();
            requirement.RecordNumberIsNaValues(fieldIds, requirementDefinition);
        }

        private static void RecordCheckBoxValues(List<CheckBoxFieldValue> values, TagRequirement requirement, RequirementDefinition requirementDefinition)
        {
            var checkBoxValues =
                values.ToDictionary(keySelector => keySelector.FieldId, elementSelector => elementSelector.IsChecked);
            requirement.RecordCheckBoxValues(checkBoxValues, requirementDefinition);
        }
    }
}
