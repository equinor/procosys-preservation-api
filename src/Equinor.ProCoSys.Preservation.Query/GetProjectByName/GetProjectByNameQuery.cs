using Equinor.ProCoSys.Common;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetProjectByName
{
    public class GetProjectByNameQuery : IRequest<Result<ProjectDetailsDto>>, IProjectRequest
    {
        public GetProjectByNameQuery(string projectName) => ProjectName = projectName;

        public string ProjectName { get; }
    }
}
