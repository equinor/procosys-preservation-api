using System.Collections.Generic;
using Equinor.ProCoSys.Preservation.Domain;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetUniqueTagModes
{
    public class GetUniqueTagModesQuery : IRequest<Result<List<ModeDto>>>, IProjectRequest
    {
        public GetUniqueTagModesQuery(string projectName) => ProjectName = projectName;

        public string ProjectName { get; }
    }
}
