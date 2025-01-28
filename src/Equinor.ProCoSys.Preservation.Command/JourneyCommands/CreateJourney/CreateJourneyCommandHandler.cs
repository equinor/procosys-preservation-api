using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Command.Services.ProjectImportService;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.JourneyCommands.CreateJourney
{
    public class CreateJourneyCommandHandler : IRequestHandler<CreateJourneyCommand, Result<int>>
    {
        private readonly IJourneyRepository _journeyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlantProvider _plantProvider;
        private readonly IProjectImportService _projectImportService;

        public CreateJourneyCommandHandler(IJourneyRepository journeyRepository, IUnitOfWork unitOfWork,
            IPlantProvider plantProvider,
            IProjectImportService projectImportService)
        {
            _journeyRepository = journeyRepository;
            _unitOfWork = unitOfWork;
            _plantProvider = plantProvider;
            _projectImportService = projectImportService;
        }

        public async Task<Result<int>> Handle(CreateJourneyCommand request, CancellationToken cancellationToken)
        {
            Project project = null;
            if (request.ProjectName is not null)
            {
                project = await _projectImportService.TryGetOrImportProjectAsync(request.ProjectName, cancellationToken);
                if (project is null)
                {
                    return new NotFoundResult<int>("Project not found");
                }
            }

            var newJourney = new Journey(_plantProvider.Plant, request.Title, project);

            _journeyRepository.Add(newJourney);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<int>(newJourney.Id);
        }
    }
}
