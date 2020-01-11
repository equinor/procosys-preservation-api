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
            var journey = await _journeyRepository.GetByIdAsync(request.JourneyId);
            if (journey == null)
            {
                return new NotFoundResult<int>(Strings.EntityNotFound(nameof(Journey), request.JourneyId));
            }

            var requirements = new List<Requirement>();
            foreach (var requirementDefinitionId in request.RequirementDefinitionIds)
            {
                var requirementDefinition = await _requirementTypeRepository.GetRequirementDefinitionByIdAsync(requirementDefinitionId);
                if (requirementDefinition == null)
                {
                    return new NotFoundResult<int>(Strings.EntityNotFound(nameof(RequirementDefinition),
                        requirementDefinitionId));
                }

                requirements.Add(new Requirement(_plantProvider.Plant, requirementDefinition.DefaultInterval, requirementDefinition));
            }

            var result = await _tagApiService.GetTags(_plantProvider.Plant, "1"); //TODO: Use this to enrich the tag.

            var tagToAdd = new Tag(_plantProvider.Plant, request.TagNo, request.ProjectNo, journey.Steps.FirstOrDefault(step => step.Id == request.StepId), requirements);
            _tagRepository.Add(tagToAdd);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<int>(tagToAdd.Id);
        }
    }
}
