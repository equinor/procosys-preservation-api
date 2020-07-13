using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.RequirementTypeCommands.DeleteRequirementDefinition
{
    public class DeleteRequirementDefinitionCommandHandler : IRequestHandler<DeleteRequirementDefinitionCommand, Result<Unit>>
    {
        private readonly IRequirementTypeRepository _requirementTypeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteRequirementDefinitionCommandHandler(IRequirementTypeRepository requirementTypeRepository, IUnitOfWork unitOfWork)
        {
            _requirementTypeRepository = requirementTypeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Unit>> Handle(DeleteRequirementDefinitionCommand request, CancellationToken cancellationToken)
        {
            var requirementType = await _requirementTypeRepository.GetByIdAsync(request.RequirementTypeId);

            var requirementDefinition =
                requirementType.RequirementDefinitions.Single(rd => rd.Id == request.RequirementDefinitionId);
            requirementDefinition.SetRowVersion(request.RowVersion);

            requirementType.RemoveRequirementDefinition(requirementDefinition);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<Unit>(Unit.Value);
        }
    }
}
