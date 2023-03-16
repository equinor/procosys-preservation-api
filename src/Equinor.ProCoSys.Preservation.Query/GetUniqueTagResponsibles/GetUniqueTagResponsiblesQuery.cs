using System.Collections.Generic;
using Equinor.ProCoSys.Common;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetUniqueTagResponsibles
{
    public class GetUniqueTagResponsiblesQuery : IRequest<Result<List<ResponsibleDto>>>, IProjectRequest
    {
        public GetUniqueTagResponsiblesQuery(string projectName) => ProjectName = projectName;

        public string ProjectName { get; }
    }
}
