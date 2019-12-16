using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.Domain.Exceptions;
using MediatR;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands
{
    public class CreateStepCommandHandler : IRequestHandler<CreateStepCommand, Unit>
    {
        private readonly IJourneyRepository _journeyRepository;
        private readonly IModeRepository _modeRepository;
        private readonly IResponsibleRepository _responsibleRepository;
        private readonly IPlantProvider _plantProvider;

        public CreateStepCommandHandler(
            IJourneyRepository journeyRepository,
            IModeRepository modeRepository,
            IResponsibleRepository responsibleRepository,
            IPlantProvider plantProvider)
        {
            _journeyRepository = journeyRepository;
            _modeRepository = modeRepository;
            _responsibleRepository = responsibleRepository;
            _plantProvider = plantProvider;
        }

        public async Task<Unit> Handle(CreateStepCommand request, CancellationToken cancellationToken)
        {
            var journey = await _journeyRepository.GetByIdAsync(request.JourneyId);
            if (journey == null)
                throw new ProcosysEntityNotFoundException($"{nameof(Journey)} with ID {request.JourneyId} not found");
            var mode = await _modeRepository.GetByIdAsync(request.ModeId);
            if (mode == null)
                throw new ProcosysEntityNotFoundException($"{nameof(Mode)} with ID {request.ModeId} not found");
            var responsible = await _responsibleRepository.GetByIdAsync(request.ResponsibleId);
            if (responsible == null)
                throw new ProcosysEntityNotFoundException($"{nameof(Responsible)} with ID {request.ResponsibleId} not found");

            journey.AddStep(new Step(_plantProvider.Plant, mode, responsible));
            await _journeyRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
