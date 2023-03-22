using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Command.ProjectCommands.FillPCSGuids;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.MainApi.Project;
using Equinor.ProCoSys.Preservation.MainApi.Tag;
using MediatR;
using Microsoft.Extensions.Logging;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.ProjectCommands.FillPCSGuids
{
    public class FillPCSGuidsCommandHandler : IRequestHandler<FillPCSGuidsCommand, Result<Unit>>
    {
        private readonly ILogger<FillPCSGuidsCommand> _logger;
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectApiService _projectApiService;
        private readonly IPlantProvider _plantProvider;
        private readonly IUnitOfWork _unitOfWork;

        public FillPCSGuidsCommandHandler(
            ILogger<FillPCSGuidsCommand> logger,
            IPlantProvider plantProvider,
            IProjectRepository projectRepository,
            IProjectApiService projectApiService,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _plantProvider = plantProvider;
            _projectRepository = projectRepository;
            _projectApiService = projectApiService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Unit>> Handle(FillPCSGuidsCommand request, CancellationToken cancellationToken)
        {
            var allProjects = await _projectRepository.GetAllAsync();
            var count = 0;
            foreach (var project in allProjects)
            {
                if(project.ProCoSysGuid == Guid.Empty)
                {
                    var pcsProjectDetails = await _projectApiService.TryGetProjectAsync(project.Plant, project.Name);
                    if(pcsProjectDetails != null)
                    {
                        project.ProCoSysGuid = pcsProjectDetails.ProCoSysGuid;
                        count++;
                    }
                }
                _logger.LogInformation($"FillPCSGuids: Project updated: {project.Name}");
            }

            if (request.SaveChanges && count > 0)
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            return new SuccessResult<Unit>(Unit.Value);
        }
    }
}
