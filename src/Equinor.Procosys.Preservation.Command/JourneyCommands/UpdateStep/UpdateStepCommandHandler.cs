using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.UpdateStep
{
    public class UpdateStepCommandHandler : IRequestHandler<UpdateStepCommand, Result<string>>
    {
        private readonly IJourneyRepository _journeyRepository;
        private readonly IModeRepository _modeRepository;
        private readonly IResponsibleRepository _responsibleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateStepCommandHandler(
            IJourneyRepository journeyRepository, 
            IModeRepository modeRepository, 
            IResponsibleRepository responsibleRepository,
            IUnitOfWork unitOfWork)
        {
            _journeyRepository = journeyRepository;
            _unitOfWork = unitOfWork;
            _modeRepository = modeRepository;
            _responsibleRepository = responsibleRepository;
        }

        public async Task<Result<string>> Handle(UpdateStepCommand request, CancellationToken cancellationToken)
        {
            var journey = await _journeyRepository.GetByIdAsync(request.JourneyId);
            var step = journey.Steps.Single(s => s.Id == request.StepId);
            var mode = await _modeRepository.GetByIdAsync(request.ModeId);
            var responsible = await _responsibleRepository.GetByIdAsync(request.ResponsibleId);

            step.SetMode(mode);
            step.SetResponsible(responsible);
            step.Title = request.Title;
            step.SetRowVersion(request.RowVersion);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<string>(step.RowVersion.ConvertToString());
        }
    }
}
