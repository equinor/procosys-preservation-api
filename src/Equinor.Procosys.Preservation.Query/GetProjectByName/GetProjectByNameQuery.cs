using Equinor.Procosys.Preservation.Domain;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetProjectByName
{
    public class GetProjectByNameQuery : IRequest<Result<ProjectDetailsDto>>, IProjectRequest
    {
        public GetProjectByNameQuery(string projectName) => ProjectName = projectName;

        public string ProjectName { get; }
    }
}
