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

    public async Task<Project> TryGetOrImportProjectAsync(string projectName)
    {
        var project = await _projectRepository.GetProjectOnlyByNameAsync(projectName);
        if (project == null)
        {
            project = await TryGetProjectFromPCS(projectName);
        }

        return project;
    }

    private async Task<Project> TryGetProjectFromPCS(string projectName)
    {
        var mainProject = await _projectApiService.TryGetProjectAsync(_plantProvider.Plant, projectName);
        if (mainProject == null)
        {
            return null;
        }

        var project = new Project(_plantProvider.Plant, projectName, mainProject.Description, mainProject.ProCoSysGuid);
        _projectRepository.Add(project);
        return project;
    }
}
