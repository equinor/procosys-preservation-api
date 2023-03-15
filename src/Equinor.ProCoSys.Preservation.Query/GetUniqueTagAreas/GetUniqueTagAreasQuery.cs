using System.Collections.Generic;
using Equinor.ProCoSys.Common;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetUniqueTagAreas
{
    public class GetUniqueTagAreasQuery : IRequest<Result<List<AreaDto>>>, IProjectRequest
    {
        public GetUniqueTagAreasQuery(string projectName) => ProjectName = projectName;

        public string ProjectName { get; }
    }
}
