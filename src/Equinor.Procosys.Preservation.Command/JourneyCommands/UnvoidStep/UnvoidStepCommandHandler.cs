using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.UnvoidStep
{
    public class UnvoidStepCommandHandler : IRequestHandler<UnvoidStepCommand, Result<string>>
    {
        private readonly IJourneyRepository _journeyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UnvoidStepCommandHandler(IJourneyRepository journeyRepository, IUnitOfWork unitOfWork)
        {
            _journeyRepository = journeyRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> Handle(UnvoidStepCommand request, CancellationToken cancellationToken)
        {
            var journey = await _journeyRepository.GetByIdAsync(request.JourneyId);

            var step = journey.UnvoidStep(request.StepId, request.RowVersion);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new SuccessResult<string>(step.RowVersion.ConvertToString());
        }
    }
}
