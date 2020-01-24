using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.Procosys.Preservation.Command.Validators.Project
{
    public class ProjectValidator : IProjectValidator
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectValidator(IProjectRepository projectRepository)
            => _projectRepository = projectRepository;

        public bool Exists(string projectName)
            => _projectRepository.GetByNameAsync(projectName).Result != null;

        public bool IsClosed(string projectName)
        {
            var project = _projectRepository.GetByNameAsync(projectName).Result;

            return project != null && project.IsClosed;
        }
    }
}
