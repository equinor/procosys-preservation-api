using Equinor.Procosys.Preservation.Domain.ProjectAccess;

namespace Equinor.Procosys.Preservation.WebApi.ProjectAccess
{
    public class ProjectAccessFailure
    {
        public ProjectAccessFailure(string projectName, PathToProjectType pathToProjectType)
        {
            ProjectName = projectName;
            PathToProjectType = pathToProjectType;
        }

        public string ProjectName { get; }
        public PathToProjectType PathToProjectType { get; }
    }
}
