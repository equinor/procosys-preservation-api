using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.UnvoidJourney
{
    public class UnvoidJourneyCommandHandler : IRequestHandler<UnvoidJourneyCommand, Result<string>>
    {
        private readonly IJourneyRepository _journeyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UnvoidJourneyCommandHandler(IJourneyRepository journeyRepository, IUnitOfWork unitOfWork)
        {
            _journeyRepository = journeyRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> Handle(UnvoidJourneyCommand request, CancellationToken cancellationToken)
        {
            var journey = await _journeyRepository.GetByIdAsync(request.JourneyId);

            journey.UnVoid();
            journey.SetRowVersion(request.RowVersion);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<string>(journey.RowVersion.ConvertToString());
        }
    }
}
