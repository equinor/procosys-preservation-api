using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Auth.Misc;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.RequirementTypeCommands.CreateRequirementDefinition
{
    public class CreateRequirementDefinitionCommandHandler : IRequestHandler<CreateRequirementDefinitionCommand, Result<int>>
    {
        private readonly IRequirementTypeRepository _requirementTypeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlantProvider _plantProvider;

        public CreateRequirementDefinitionCommandHandler(
            IRequirementTypeRepository requirementTypeRepository,
            IUnitOfWork unitOfWork,
            IPlantProvider plantProvider)
        {
            _requirementTypeRepository = requirementTypeRepository;
            _unitOfWork = unitOfWork;
            _plantProvider = plantProvider;
        }

        public async Task<Result<int>> Handle(CreateRequirementDefinitionCommand request, CancellationToken cancellationToken)
        {
            var requirementType = await _requirementTypeRepository.GetByIdAsync(request.RequirementTypeId);
            var newRequirementDefinition = new RequirementDefinition(
                _plantProvider.Plant,
                request.Title,
                request.DefaultIntervalWeeks,
                request.Usage,
                request.SortKey);
            requirementType.AddRequirementDefinition(newRequirementDefinition);

            foreach (var field in request.Fields)
            {
                newRequirementDefinition.AddField(new Field(
                    _plantProvider.Plant,
                    field.Label,
                    field.FieldType,
                    field.SortKey,
                    field.Unit,
                    field.ShowPrevious));
            }
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new SuccessResult<int>(newRequirementDefinition.Id);
        }
    }
}
