using System.Collections.Generic;
using Equinor.ProCoSys.Common;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetUniqueTagDisciplines
{
    public class GetUniqueTagDisciplinesQuery : IRequest<Result<List<DisciplineDto>>>, IProjectRequest
    {
        public GetUniqueTagDisciplinesQuery(string projectName) => ProjectName = projectName;

        public string ProjectName { get; }
    }
}
