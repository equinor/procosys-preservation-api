using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.RequirementTypeCommands.CreateRequirementDefinition
{
    public class CreateRequirementDefinitionCommandHandler : IRequestHandler<CreateRequirementDefinitionCommand, Result<int>>
    {
        private readonly IRequirementTypeRepository _requirementTypeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlantProvider _plantProvider;

        public CreateRequirementDefinitionCommandHandler(IRequirementTypeRepository requirementTypeRepository,
            IUnitOfWork unitOfWork, IPlantProvider plantProvider)
        {
            _requirementTypeRepository = requirementTypeRepository;
            _unitOfWork = unitOfWork;
            _plantProvider = plantProvider;
        }

        public async Task<Result<int>> Handle(CreateRequirementDefinitionCommand request, CancellationToken cancellationToken)
        {
            var requirementType = await _requirementTypeRepository.GetByIdAsync(request.RequirementTypeId);
            var newRequirementDefinition = new RequirementDefinition(_plantProvider.Plant, request.Title,
                request.DefaultIntervalWeeks, request.Usage, request.SortKey);
            requirementType.AddRequirementDefinition(newRequirementDefinition);

            foreach (var field in request.Fields ?? Enumerable.Empty<Field>())
            {
                newRequirementDefinition.AddField(field);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new SuccessResult<int>(newRequirementDefinition.Id);
        }
    }
}
