using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.UpdateStep
{
    public class UpdateStepCommandHandler : IRequestHandler<UpdateStepCommand, Result<int>>
    {
        private readonly IStepRepository _stepRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlantProvider _plantProvider;

        public UpdateStepCommandHandler(IStepRepository stepRepository, IUnitOfWork unitOfWork, IPlantProvider plantProvider)
        {
            _stepRepository = stepRepository;
            _unitOfWork = unitOfWork;
            _plantProvider = plantProvider;
        }

        public async Task<Result<int>> Handle(UpdateStepCommand request, CancellationToken cancellationToken)
        {
            var step = await _stepRepository.GetStepByStepIdAsync(request.StepId);

            step.UpdateStep(step, request.Title);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<int>(step.Id);
        }
    }
}
