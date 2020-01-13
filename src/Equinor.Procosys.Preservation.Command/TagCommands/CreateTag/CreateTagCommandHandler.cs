using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
using Equinor.Procosys.Preservation.MainApi;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.CreateTag
{
    public class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, Result<int>>
    {
        private readonly ITagRepository _tagRepository;
        private readonly IJourneyRepository _journeyRepository;
        private readonly IRequirementTypeRepository _requirementTypeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlantProvider _plantProvider;
        private readonly ITagApiService _tagApiService;

        public CreateTagCommandHandler(
            ITagRepository tagRepository,
            IJourneyRepository journeyRepository,
            IRequirementTypeRepository requirementTypeRepository,
            IUnitOfWork unitOfWork,
            IPlantProvider plantProvider,
            ITagApiService tagApiService)
        {
            _tagRepository = tagRepository;
            _journeyRepository = journeyRepository;
            _requirementTypeRepository = requirementTypeRepository;
            _unitOfWork = unitOfWork;
            _plantProvider = plantProvider;
            _tagApiService = tagApiService;
        }

        public async Task<Result<int>> Handle(CreateTagCommand request, CancellationToken cancellationToken)
        {
            var step = await _journeyRepository.GetStepByStepIdAsync(request.StepId);
            if (step == null)
            {
                return new NotFoundResult<int>(Strings.EntityNotFound(nameof(Step), request.StepId));
            }

            var requirements = new List<Requirement>();
            foreach (var requirement in request.Requirements)
            {
                var requirementDefinition =
                    await _requirementTypeRepository.GetRequirementDefinitionByIdAsync(requirement.RequirementDefinitionId);
                if (requirementDefinition == null)
                {
                    return new NotFoundResult<int>(Strings.EntityNotFound(
                        nameof(RequirementDefinition),
                        requirement.RequirementDefinitionId));
                }

                requirements.Add(new Requirement(_plantProvider.Plant, requirement.Interval, requirementDefinition));
            }

            var result = await _tagApiService.GetTags(_plantProvider.Plant, "1"); //TODO: Use this to enrich the tag.

            var tagToAdd = new Tag(_plantProvider.Plant, 
                request.TagNo, 
                request.ProjectNo,
                step,
                requirements);
            _tagRepository.Add(tagToAdd);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<int>(tagToAdd.Id);
        }
    }
}
