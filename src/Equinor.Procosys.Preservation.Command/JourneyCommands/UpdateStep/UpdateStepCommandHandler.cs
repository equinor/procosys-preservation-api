using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.MainApi.Responsible;
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
        private readonly IPlantProvider _plantProvider;
        private readonly IResponsibleApiService _responsibleApiService;

        public UpdateStepCommandHandler(
            IJourneyRepository journeyRepository, 
            IModeRepository modeRepository, 
            IResponsibleRepository responsibleRepository,
            IUnitOfWork unitOfWork,
            IPlantProvider plantProvider,
            IResponsibleApiService responsibleApiService)
        {
            _journeyRepository = journeyRepository;
            _unitOfWork = unitOfWork;
            _modeRepository = modeRepository;
            _responsibleRepository = responsibleRepository;
            _plantProvider = plantProvider;
            _responsibleApiService = responsibleApiService;
        }

        public async Task<Result<string>> Handle(UpdateStepCommand request, CancellationToken cancellationToken)
        {
            var journey = await _journeyRepository.GetByIdAsync(request.JourneyId);
            var step = journey.Steps.Single(s => s.Id == request.StepId);
            var mode = await _modeRepository.GetByIdAsync(request.ModeId);

            var responsible = await _responsibleRepository.GetByCodeAsync(request.ResponsibleCode);

            if (responsible == null)
            {
                responsible = await CreateResponsibleAsync(request.ResponsibleCode);
                if (responsible == null)
                {
                    return new NotFoundResult<string>($"Responsible with code {request.ResponsibleCode} not found");
                }
                // must save new Responsible to get id of it
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            step.SetMode(mode);
            step.SetResponsible(responsible);
            step.Title = request.Title;
            step.SetRowVersion(request.RowVersion);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<string>(step.RowVersion.ConvertToString());
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
