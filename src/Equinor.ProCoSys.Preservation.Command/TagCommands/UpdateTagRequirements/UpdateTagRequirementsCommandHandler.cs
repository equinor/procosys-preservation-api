using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.TagCommands.UpdateTagRequirements
{
    public class UpdateTagRequirementsCommandHandler : IRequestHandler<UpdateTagRequirementsCommand, Result<string>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IRequirementTypeRepository _requirementTypeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlantProvider _plantProvider;

        public UpdateTagRequirementsCommandHandler(
            IProjectRepository projectRepository,
            IRequirementTypeRepository requirementTypeRepository,
            IUnitOfWork unitOfWork,
            IPlantProvider plantProvider)
        {
            _projectRepository = projectRepository;
            _requirementTypeRepository = requirementTypeRepository;
            _unitOfWork = unitOfWork;
            _plantProvider = plantProvider;
        }

        public async Task<Result<string>> Handle(UpdateTagRequirementsCommand request, CancellationToken cancellationToken)
        {
            var tag = await _projectRepository.GetTagWithPreservationHistoryByTagIdAsync(request.TagId);

            if (tag.IsAreaTag() && request.Description != null)
            {
                tag.Description = request.Description;
            }

            foreach (var update in request.UpdatedRequirements)
            {
                tag.UpdateRequirement(update.TagRequirementId, update.IsVoided, update.IntervalWeeks, update.RowVersion);
            }

            foreach (var delete in request.DeletedRequirements)
            {
                tag.RemoveRequirement(delete.TagRequirementId, delete.RowVersion);
            }

            foreach (var newRequirement in request.NewRequirements)
            {
                var reqDef = await _requirementTypeRepository.GetRequirementDefinitionByIdAsync(newRequirement.RequirementDefinitionId);
                tag.AddRequirement(new TagRequirement(_plantProvider.Plant, newRequirement.IntervalWeeks, reqDef));
            }

            tag.SetRowVersion(request.RowVersion);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<string>(tag.RowVersion.ConvertToString());
        }
    }
}
