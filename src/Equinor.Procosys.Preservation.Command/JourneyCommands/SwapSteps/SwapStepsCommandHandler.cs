using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.SwapSteps
{
    public class SwapStepsCommandHandler : IRequestHandler<SwapStepsCommand, Result<string>>
    {
        private readonly IJourneyRepository _journeyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SwapStepsCommandHandler(IJourneyRepository journeyRepository, IUnitOfWork unitOfWork)
        {
            _journeyRepository = journeyRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> Handle(SwapStepsCommand request, CancellationToken cancellationToken)
        {
            var journey = await _journeyRepository.GetByIdAsync(request.JourneyId);
            var stepA = journey.Steps.Single(s => s.Id == request.StepAId);
            var stepB = journey.Steps.Single(s => s.Id == request.StepBId);

            // Swapping the SortKeys
            var tempSortKey = stepA.SortKey;
            stepA.SortKey = stepB.SortKey;
            stepB.SortKey = tempSortKey;

            stepA.SetRowVersion(request.RowVersionA);
            stepB.SetRowVersion(request.RowVersionB);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<string>("RowVersionA: " + stepA.RowVersion.ConvertToString() + ", " + "RowVersionB: " + stepB.RowVersion.ConvertToString());
        }
    }
}
