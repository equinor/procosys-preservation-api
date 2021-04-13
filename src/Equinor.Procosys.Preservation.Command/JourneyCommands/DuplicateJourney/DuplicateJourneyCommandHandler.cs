using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.JourneyCommands.DuplicateJourney
{
    public class DuplicateJourneyCommandHandler : IRequestHandler<DuplicateJourneyCommand, Result<int>>
    {
        private readonly IJourneyRepository _journeyRepository;
        private readonly IModeRepository _modeRepository;
        private readonly IResponsibleRepository _responsibleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlantProvider _plantProvider;

        public DuplicateJourneyCommandHandler(
            IJourneyRepository journeyRepository,
            IModeRepository modeRepository,
            IResponsibleRepository responsibleRepository,
            IUnitOfWork unitOfWork,
            IPlantProvider plantProvider)
        {
            _journeyRepository = journeyRepository;
            _modeRepository = modeRepository;
            _responsibleRepository = responsibleRepository;
            _unitOfWork = unitOfWork;
            _plantProvider = plantProvider;
        }

        public async Task<Result<int>> Handle(DuplicateJourneyCommand request, CancellationToken cancellationToken)
        {
            var journey = await _journeyRepository.GetByIdAsync(request.JourneyId);
            
            var responsibleIds = journey.Steps.Select(s => s.ResponsibleId).Distinct();
            var modeIds = journey.Steps.Select(s => s.ModeId).Distinct();
            
            var responsibles = await  _responsibleRepository.GetByIdsAsync(responsibleIds);
            var modes = await _modeRepository.GetByIdsAsync(modeIds);

            var plant = _plantProvider.Plant;
            var newJourney = new Journey(plant, $"{journey.Title}{Journey.DuplicatePrefix}");

            foreach (var step in journey.OrderedSteps())
            {
                var responsible = responsibles.Single(r => r.Id == step.ResponsibleId);
                var mode = modes.Single(m => m.Id == step.ModeId);
                var newStep = new Step(plant, step.Title, mode, responsible);
                newJourney.AddStep(newStep);
            }

            _journeyRepository.Add(newJourney);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<int>(newJourney.Id);
        }
    }
}
