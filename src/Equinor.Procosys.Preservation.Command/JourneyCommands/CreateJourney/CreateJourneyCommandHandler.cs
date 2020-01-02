using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using MediatR;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.CreateJourney
{
    public class CreateJourneyCommandHandler : IRequestHandler<CreateJourneyCommand, int>
    {
        private readonly IJourneyRepository _journeyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlantProvider _plantProvider;

        public CreateJourneyCommandHandler(IJourneyRepository journeyRepository, IUnitOfWork unitOfWork, IPlantProvider plantProvider)
        {
            _journeyRepository = journeyRepository;
            _unitOfWork = unitOfWork;
            _plantProvider = plantProvider;
        }

        public async Task<int> Handle(CreateJourneyCommand request, CancellationToken cancellationToken)
        {
            var newJourney = new Journey(_plantProvider.Plant, request.Title);

            _journeyRepository.Add(newJourney);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return newJourney.Id;
        }
    }
}
