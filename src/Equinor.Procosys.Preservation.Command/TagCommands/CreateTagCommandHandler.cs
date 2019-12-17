using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.MainApi;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
using Equinor.Procosys.Preservation.Domain.Exceptions;
using MediatR;

namespace Equinor.Procosys.Preservation.Command.TagCommands
{
    public class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, int>
    {
        private readonly ITagRepository _tagRepository;
        private readonly IJourneyRepository _journeyRepository;
        private readonly IPlantProvider _plantProvider;
        private readonly IMainApiService _mainApiService;

        public CreateTagCommandHandler(ITagRepository tagRepository, IJourneyRepository journeyRepository, IPlantProvider plantProvider, IMainApiService mainApiService)
        {
            _tagRepository = tagRepository;
            _journeyRepository = journeyRepository;
            _plantProvider = plantProvider;
            _mainApiService = mainApiService;
        }

        public async Task<int> Handle(CreateTagCommand request, CancellationToken cancellationToken)
        {
            var journey = await _journeyRepository.GetByIdAsync(request.JourneyId);
            if (journey == null)
            {
                throw new ProcosysEntityNotFoundException($"{nameof(Journey)} with ID {request.JourneyId} not found");
            }

            var result = await _mainApiService.GetTags("1");

            var tagToAdd = new Tag(_plantProvider.Plant, request.TagNo, request.ProjectNo, journey.Steps.FirstOrDefault(step => step.Id == request.StepId));
            _tagRepository.Add(tagToAdd);
            await _tagRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return tagToAdd.Id;
        }
    }
}
