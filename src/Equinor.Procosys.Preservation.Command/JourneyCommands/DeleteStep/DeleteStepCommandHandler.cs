using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.DeleteStep
{
    public class DeleteStepCommandHandler : IRequestHandler<DeleteStepCommand, Result<Unit>>
    {
        private readonly IJourneyRepository _journeyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteStepCommandHandler(IJourneyRepository journeyRepository, IUnitOfWork unitOfWork)
        {
            _journeyRepository = journeyRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Unit>> Handle(DeleteStepCommand request, CancellationToken cancellationToken)
        {
            var journey = await _journeyRepository.GetByIdAsync(request.JourneyId);
            var step = journey.Steps.Single(s => s.Id == request.StepId);

            step.SetRowVersion(request.RowVersion);
            journey.RemoveStep(step);
            _journeyRepository.RemoveStep(step);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new SuccessResult<Unit>(Unit.Value);
        }
    }
}
