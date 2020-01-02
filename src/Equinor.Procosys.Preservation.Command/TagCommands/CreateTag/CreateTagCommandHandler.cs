using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
using Equinor.Procosys.Preservation.MainApi;
using MediatR;

namespace Equinor.Procosys.Preservation.Command.TagCommands.CreateTag
{
    public class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, int>
    {
        private readonly ITagRepository _tagRepository;
        private readonly IJourneyRepository _journeyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlantProvider _plantProvider;
        private readonly IMainApiService _mainApiService;

        public CreateTagCommandHandler(
            ITagRepository tagRepository,
            IJourneyRepository journeyRepository,
            IUnitOfWork unitOfWork,
            IPlantProvider plantProvider,
            IMainApiService mainApiService)
        {
            _tagRepository = tagRepository;
            _journeyRepository = journeyRepository;
            _unitOfWork = unitOfWork;
            _plantProvider = plantProvider;
            _mainApiService = mainApiService;
        }

        public async Task<int> Handle(CreateTagCommand request, CancellationToken cancellationToken)
        {
            var journey = await _journeyRepository.GetByIdAsync(request.JourneyId);
            var result = await _mainApiService.GetTags(_plantProvider.Plant, "1");

            var tagToAdd = new Tag(_plantProvider.Plant, request.TagNo, request.ProjectNo, journey.Steps.FirstOrDefault(step => step.Id == request.StepId));
            _tagRepository.Add(tagToAdd);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return tagToAdd.Id;
        }
    }
}
