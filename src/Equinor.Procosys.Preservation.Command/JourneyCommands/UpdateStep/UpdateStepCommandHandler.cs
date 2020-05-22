using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.UpdateStep
{
    public class UpdateStepCommandHandler : IRequestHandler<UpdateStepCommand, Result<string>>
    {
        private readonly IJourneyRepository _journeyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IModeRepository _modeRepository;
        private readonly IPlantProvider _plantProvider;

        public UpdateStepCommandHandler(
            IJourneyRepository journeyRepository, 
            IModeRepository modeRepository, 
            IUnitOfWork unitOfWork,
            IPlantProvider plantProvider)
        {
            _journeyRepository = journeyRepository;
            _unitOfWork = unitOfWork;
            _modeRepository = modeRepository;
            _plantProvider = plantProvider;
        }

        public async Task<Result<string>> Handle(UpdateStepCommand request, CancellationToken cancellationToken)
        {
            var journey = await _journeyRepository.GetByIdAsync(request.JourneyId);
            var step = journey.Steps.Single(s => s.Id == request.StepId);
            var mode = await _modeRepository.GetByIdAsync(request.ModeId) ?? new Mode(_plantProvider.Plant, "a test mode");//what should be mode title if we create?/ Should it fail if not exists?

            step.SetMode(mode);
            //TODO  set responsible Id
            step.Title = request.Title;
            step.SetRowVersion(request.RowVersion);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<string>(step.RowVersion.ConvertToString());
        }
    }
}
