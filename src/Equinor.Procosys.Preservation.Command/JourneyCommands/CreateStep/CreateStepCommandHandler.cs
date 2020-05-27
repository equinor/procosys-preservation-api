using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.MainApi.Responsible;
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
        private readonly IResponsibleApiService _responsibleApiService;

        public CreateStepCommandHandler(
            IJourneyRepository journeyRepository,
            IModeRepository modeRepository,
            IResponsibleRepository responsibleRepository,
            IUnitOfWork unitOfWork,
            IPlantProvider plantProvider,
            IResponsibleApiService responsibleApiService)
        {
            _journeyRepository = journeyRepository;
            _modeRepository = modeRepository;
            _responsibleRepository = responsibleRepository;
            _unitOfWork = unitOfWork;
            _plantProvider = plantProvider;
            _responsibleApiService = responsibleApiService;
        }

        public async Task<Result<Unit>> Handle(CreateStepCommand request, CancellationToken cancellationToken)
        {
            var journey = await _journeyRepository.GetByIdAsync(request.JourneyId);
            var mode = await _modeRepository.GetByIdAsync(request.ModeId);

            var responsible = await _responsibleRepository.GetByCodeAsync(request.ResponsibleCode);

            if (responsible == null)
            {
                responsible = await CreateResponsibleAsync(request.ResponsibleCode);
                if (responsible == null)
                {
                    return new NotFoundResult<Unit>($"Responsible with code {request.ResponsibleCode} not found");
                }
                // must save new Responsible to get id of it
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            journey.AddStep(new Step(_plantProvider.Plant, request.Title, mode, responsible));
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<Unit>(Unit.Value);
        }

        private async Task<Responsible> CreateResponsibleAsync(string responsibleCode)
        {
            var mainResponsible = await _responsibleApiService.GetResponsibleAsync(_plantProvider.Plant, responsibleCode);
            if (mainResponsible == null)
            {
                return null;
            }

            var responsible = new Responsible(_plantProvider.Plant, responsibleCode, mainResponsible.Description);
            _responsibleRepository.Add(responsible);
            return responsible;
        }
    }
}
