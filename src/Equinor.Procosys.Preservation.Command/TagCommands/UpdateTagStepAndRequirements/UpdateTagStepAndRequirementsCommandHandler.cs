using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.UpdateTagStepAndRequirements
{
    public class UpdateTagStepAndRequirementsCommandHandler : IRequestHandler<UpdateTagStepAndRequirementsCommand, Result<string>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IJourneyRepository _journeyRepository;
        private readonly IRequirementTypeRepository _requirementTypeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlantProvider _plantProvider;

        public UpdateTagStepAndRequirementsCommandHandler(
            IProjectRepository projectRepository, 
            IJourneyRepository journeyRepository, 
            IRequirementTypeRepository requirementTypeRepository, 
            IUnitOfWork unitOfWork, 
            IPlantProvider plantProvider)
        {
            _projectRepository = projectRepository;
            _journeyRepository = journeyRepository;
            _requirementTypeRepository = requirementTypeRepository;
            _unitOfWork = unitOfWork;
            _plantProvider = plantProvider;
        }

        public async Task<Result<string>> Handle(UpdateTagStepAndRequirementsCommand request, CancellationToken cancellationToken)
        {
            var tag = await _projectRepository.GetTagByTagIdAsync(request.TagId);
            var step = await _journeyRepository.GetStepByStepIdAsync(request.StepId);

            if (tag.IsAreaTag() && request.Description != null)
            {
                tag.Description = request.Description;
            }
            
            tag.UpdateStep(step);

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
