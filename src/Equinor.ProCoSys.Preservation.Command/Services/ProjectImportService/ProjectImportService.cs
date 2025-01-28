using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.MainApi.Project;

namespace Equinor.ProCoSys.Preservation.Command.Services.ProjectImportService;

public class ProjectImportService : IProjectImportService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IProjectApiService _projectApiService;
    private readonly IPlantProvider _plantProvider;

    public ProjectImportService(
        IProjectRepository projectRepository,
        IProjectApiService projectApiService,
        IPlantProvider plantProvider)
    {
        _projectRepository = projectRepository;
        _projectApiService = projectApiService;
        _plantProvider = plantProvider;
    }

    public async Task<Project> TryGetOrImportProjectAsync(string projectName, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetProjectOnlyByNameAsync(projectName);
        if (project == null)
        {
            project = await TryGetProjectFromPCS(projectName, cancellationToken);
        }

        return project;
    }

    private async Task<Project> TryGetProjectFromPCS(string projectName, CancellationToken cancellationToken)
    {
        var mainProject = await _projectApiService.TryGetProjectAsync(_plantProvider.Plant, projectName, cancellationToken);
        if (mainProject == null)
        {
            return null;
        }

        var project = new Project(_plantProvider.Plant, projectName, mainProject.Description, mainProject.ProCoSysGuid);
        _projectRepository.Add(project);
        return project;
    }
}
