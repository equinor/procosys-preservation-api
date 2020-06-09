using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.RequirementTypeCommands.UnvoidRequirementType
{
    public class UnvoidRequirementTypeCommandHandler : IRequestHandler<UnvoidRequirementTypeCommand, Result<string>>
    {
        private readonly IRequirementTypeRepository _requirementTypeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UnvoidRequirementTypeCommandHandler(IRequirementTypeRepository requirementTypeRepository, IUnitOfWork unitOfWork)
        {
            _requirementTypeRepository = requirementTypeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> Handle(UnvoidRequirementTypeCommand request, CancellationToken cancellationToken)
        {
            var requirementType = await _requirementTypeRepository.GetByIdAsync(request.RequirementTypeId);

            requirementType.UnVoid();
            requirementType.SetRowVersion(request.RowVersion);
            await _unitOfWork.SaveChangesAsync(request.CurrentUserOid, cancellationToken);

            return new SuccessResult<string>(requirementType.RowVersion.ConvertToString());
        }
    }
}
