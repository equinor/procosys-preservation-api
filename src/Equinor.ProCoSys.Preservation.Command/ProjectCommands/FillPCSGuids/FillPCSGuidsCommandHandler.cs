using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.MainApi.Project;
using MediatR;
using Microsoft.Extensions.Logging;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.ProjectCommands.FillPCSGuids
{
    public class FillPCSGuidsCommandHandler : IRequestHandler<FillPCSGuidsCommand, Result<Unit>>
    {
        private readonly ILogger<FillPCSGuidsCommand> _logger;
        private readonly IProjectRepository _projectRepository;
        private readonly IMainApiProjectApiForUserService _projectApiService;
        private readonly IPlantProvider _plantProvider;
        private readonly IUnitOfWork _unitOfWork;

        public FillPCSGuidsCommandHandler(
            ILogger<FillPCSGuidsCommand> logger,
            IPlantProvider plantProvider,
            IProjectRepository projectRepository,
            IMainApiProjectApiForUserService projectApiService,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _plantProvider = plantProvider;
            _projectRepository = projectRepository;
            _projectApiService = projectApiService;
            _unitOfWork = unitOfWork;
        }

        public Task<Result<Unit>> Handle(FillPCSGuidsCommand request, CancellationToken cancellationToken)
        {
            // THIS CODE WAS WRITTEN TO RUN A ONETIME TRANSFORMATION WHEN WE INTRODUCED ProCoSysGuid
            // WE KEEP THE CODE ... MAYBE WE WANT TO DO SIMILAR STUFF LATER

            //var allProjects = await _projectRepository.GetProjectsOnlyAsync();
            //var count = 0;
            //foreach (var project in allProjects)
            //{
            //    if(project.ProCoSysGuid == Guid.Empty)
            //    {
            //        var pcsProjectDetails = await _projectApiService.TryGetProjectAsync(project.Plant, project.Name);
            //        if(pcsProjectDetails != null)
            //        {
            //            project.ProCoSysGuid = pcsProjectDetails.ProCoSysGuid;
            //            _logger.LogInformation($"FillPCSGuids: Project updated: {project.Name}");
            //            count++;
            //        }
            //        else
            //        {
            //            _logger.LogInformation($"FillPCSGuids: pcsProjectDetails is NULL for {project.Plant}.{project.Name}");
            //        }
            //    }
            //    else
            //    {
            //        _logger.LogInformation($"FillPCSGuids: Project already updated: {project.Name}");
            //    }


            //}

            //if (request.SaveChanges && count > 0)
            //{
            //    await _unitOfWork.SaveChangesAsync(cancellationToken);
            //    _logger.LogInformation($"FillPCSGuids: {count} project updated");
            //}
            
            return Task.FromResult<Result<Unit>>(new SuccessResult<Unit>(Unit.Value));
        }
    }
}
