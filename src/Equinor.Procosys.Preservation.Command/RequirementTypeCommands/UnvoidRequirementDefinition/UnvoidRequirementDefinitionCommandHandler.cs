using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.RequirementTypeCommands.UnvoidRequirementDefinition
{
    public class UnvoidRequirementDefinitionCommandHandler : IRequestHandler<UnvoidRequirementDefinitionCommand, Result<string>>
    {
        private readonly IRequirementTypeRepository _requirementTypeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UnvoidRequirementDefinitionCommandHandler(IRequirementTypeRepository requirementTypeRepository, IUnitOfWork unitOfWork)
        {
            _requirementTypeRepository = requirementTypeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> Handle(UnvoidRequirementDefinitionCommand request, CancellationToken cancellationToken)
        {
            var requirementType = await _requirementTypeRepository.GetByIdAsync(request.RequirementTypeId);
            var requirementDefinition =
                requirementType.RequirementDefinitions.Single(rd => rd.Id == request.RequirementDefinitionId);

            requirementDefinition.UnVoid();
            requirementDefinition.SetRowVersion(request.RowVersion);
            await _unitOfWork.SaveChangesAsync(request.CurrentUserOid, cancellationToken);

            return new SuccessResult<string>(requirementDefinition.RowVersion.ConvertToString());
        }
    }
}
