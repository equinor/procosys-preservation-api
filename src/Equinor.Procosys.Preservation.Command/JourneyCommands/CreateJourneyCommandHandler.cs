using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using MediatR;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands
{
    public class CreateJourneyCommandHandler : IRequestHandler<CreateJourneyCommand, int>
    {
        private readonly IJourneyRepository _journeyRepository;
        private readonly IPlantProvider _plantProvider;
        private readonly IJourneyService _journeyService;

        public CreateJourneyCommandHandler(IJourneyRepository journeyRepository, IPlantProvider plantProvider, IJourneyService journeyService)
        {
            _journeyRepository = journeyRepository;
            _plantProvider = plantProvider;
            _journeyService = journeyService;
        }

        public async Task<int> Handle(CreateJourneyCommand request, CancellationToken cancellationToken)
        {
            var newJourney = new Journey(_plantProvider.Plant, request.Title);
            await _journeyService.ValidateNewJourney(newJourney);

            _journeyRepository.Add(newJourney);
            await _journeyRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return newJourney.Id;
        }
    }
}
