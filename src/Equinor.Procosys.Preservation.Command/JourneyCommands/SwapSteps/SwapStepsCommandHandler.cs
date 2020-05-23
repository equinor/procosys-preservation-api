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
    public class SwapStepsCommandHandler : IRequestHandler<SwapStepsCommand, Result<IEnumerable<StepIdAndRowVersion>>>
    {
        private readonly IJourneyRepository _journeyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SwapStepsCommandHandler(IJourneyRepository journeyRepository, IUnitOfWork unitOfWork)
        {
            _journeyRepository = journeyRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IEnumerable<StepIdAndRowVersion>>> Handle(SwapStepsCommand request, CancellationToken cancellationToken)
        {
            var journey = await _journeyRepository.GetByIdAsync(request.JourneyId);
            var stepA = journey.Steps.Single(s => s.Id == request.StepAId);
            var stepB = journey.Steps.Single(s => s.Id == request.StepBId);
            var stepsWithUpdatedRowVersion = new List<StepIdAndRowVersion>();

            journey.SwapSteps(stepA.Id, stepB.Id);

            stepA.SetRowVersion(request.StepARowVersion);
            stepB.SetRowVersion(request.StepBRowVersion);

            stepsWithUpdatedRowVersion.Add(new StepIdAndRowVersion(stepA.Id, stepA.RowVersion.ConvertToString()));
            stepsWithUpdatedRowVersion.Add(new StepIdAndRowVersion(stepB.Id, stepB.RowVersion.ConvertToString()));

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<IEnumerable<StepIdAndRowVersion>>(stepsWithUpdatedRowVersion);
        }
    }
}
