using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.RequirementTypeCommands.CreateRequirementType
{
    public class CreateRequirementTypeCommandHandler : IRequestHandler<CreateRequirementTypeCommand, Result<int>>
    {
        private readonly IRequirementTypeRepository _requirementTypeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlantProvider _plantProvider;

        public CreateRequirementTypeCommandHandler(IRequirementTypeRepository modeRepository, IUnitOfWork unitOfWork, IPlantProvider plantProvider)
        {
            _requirementTypeRepository = modeRepository;
            _unitOfWork = unitOfWork;
            _plantProvider = plantProvider;
        }

        public async Task<Result<int>> Handle(CreateRequirementTypeCommand request, CancellationToken cancellationToken)
        {
            var newRequirementType = new RequirementType(_plantProvider.Plant, request.Code, request.Title, request.SortKey);
            _requirementTypeRepository.Add(newRequirementType);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new SuccessResult<int>(newRequirementType.Id);
        }
    }
}
