using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.RequirementTypeCommands.UpdateRequirementDefinition
{
    public class UpdateRequirementDefinitionCommandHandler : IRequestHandler<UpdateRequirementDefinitionCommand, Result<string>>
    {
        private readonly IRequirementTypeRepository _requirementTypeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateRequirementDefinitionCommandHandler(IRequirementTypeRepository requirementTypeRepository, IUnitOfWork unitOfWork)
        {
            _requirementTypeRepository = requirementTypeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> Handle(UpdateRequirementDefinitionCommand request, CancellationToken cancellationToken)
        {
            var requirementType = await _requirementTypeRepository.GetByIdAsync(request.RequirementTypeId);

            requirementType.Title = request.Title;
            requirementType.Code = request.Code;
            requirementType.Icon = request.Icon;
            requirementType.SortKey = request.SortKey;

            requirementType.SetRowVersion(request.RowVersion);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<string>(requirementType.RowVersion.ConvertToString());
        }
    }
}
