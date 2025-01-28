using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Command.Services.ProjectImportService;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.JourneyCommands.UpdateJourney
{
    public class UpdateJourneyCommandHandler : IRequestHandler<UpdateJourneyCommand, Result<string>>
    {
        private readonly IJourneyRepository _journeyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectImportService _projectImportService;

        public UpdateJourneyCommandHandler(IJourneyRepository journeyRepository, IUnitOfWork unitOfWork,
            IProjectRepository projectRepository, IProjectImportService projectImportService)
        {
            _journeyRepository = journeyRepository;
            _unitOfWork = unitOfWork;
            _projectRepository = projectRepository;
            _projectImportService = projectImportService;
        }

        public async Task<Result<string>> Handle(UpdateJourneyCommand request, CancellationToken cancellationToken)
        {
            var journey = await _journeyRepository.GetByIdAsync(request.JourneyId);

            journey.Title = request.Title;
            journey.SetRowVersion(request.RowVersion);

            if (request.ProjectName is not null && journey.Project?.Name != request.ProjectName)
            {
                var project = await GetOrCreateProjectAsync(request);
                if (project is null)
                {
                    return new NotFoundResult<string>("Project not found");
                }

                journey.Project = project;
            }

            if (request.ProjectName is null)
            {
                journey.Project = null;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<string>(journey.RowVersion.ConvertToString());
        }

        private async Task<Project> GetOrCreateProjectAsync(UpdateJourneyCommand request)
        {
            var project = await _projectRepository.GetProjectOnlyByNameAsync(request.ProjectName);
            if (project == null)
            {
                project = await _projectImportService.ImportProjectAsync(request.ProjectName);
            }

            return project;
        }
    }
}
