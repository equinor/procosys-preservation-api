using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.ProCoSys.Preservation.MainApi.Responsible;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.JourneyCommands.CreateStep
{
    public class CreateStepCommandHandler : IRequestHandler<CreateStepCommand, Result<int>>
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

        public async Task<Result<int>> Handle(CreateStepCommand request, CancellationToken cancellationToken)
        {
            var journey = await _journeyRepository.GetByIdAsync(request.JourneyId);
            var mode = await _modeRepository.GetByIdAsync(request.ModeId);

            var responsible = await _responsibleRepository.GetByCodeAsync(request.ResponsibleCode);

            if (responsible == null)
            {
                responsible = await CreateResponsibleAsync(request.ResponsibleCode, cancellationToken);
                if (responsible == null)
                {
                    return new NotFoundResult<int>($"Responsible with code {request.ResponsibleCode} not found");
                }
                // must save new Responsible to get id of it
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            var step = new Step(_plantProvider.Plant, request.Title, mode, responsible)
            {
                AutoTransferMethod = request.AutoTransferMethod
            };

            journey.AddStep(step);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<int>(step.Id);
        }

        private async Task<Responsible> CreateResponsibleAsync(string responsibleCode, CancellationToken cancellationToken)
        {
            var mainResponsible = await _responsibleApiService.TryGetResponsibleAsync(
                _plantProvider.Plant,
                responsibleCode,
                cancellationToken);

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
