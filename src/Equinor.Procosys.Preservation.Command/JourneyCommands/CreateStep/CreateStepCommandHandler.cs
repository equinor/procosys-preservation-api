using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.JourneyCommands.CreateStep
{
    public class CreateStepCommandHandler : IRequestHandler<CreateStepCommand, Result<Unit>>
    {
        private readonly IJourneyRepository _journeyRepository;
        private readonly IModeRepository _modeRepository;
        private readonly IResponsibleRepository _responsibleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlantProvider _plantProvider;

        public CreateStepCommandHandler(
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

        public async Task<Result<Unit>> Handle(CreateStepCommand request, CancellationToken cancellationToken)
        {
            var journey = await _journeyRepository.GetByIdAsync(request.JourneyId);
            if (journey == null)
            {
                return new NotFoundResult<Unit>(Strings.EntityNotFound(nameof(Journey), request.JourneyId));
            }

            var mode = await _modeRepository.GetByIdAsync(request.ModeId);
            if (mode == null)
            {
                return new NotFoundResult<Unit>(Strings.EntityNotFound(nameof(Mode), request.ModeId));
            }

            var responsible = await _responsibleRepository.GetByIdAsync(request.ResponsibleId);
            if (responsible == null)
            {
                return new NotFoundResult<Unit>(Strings.EntityNotFound(nameof(Responsible), request.ResponsibleId));
            }

            journey.AddStep(new Step(_plantProvider.Plant, mode, responsible));
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<Unit>(Unit.Value);
        }
    }
}
