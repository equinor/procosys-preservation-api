using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetUniqueTagResponsibles
{
    public class GetUniqueTagResponsiblesQuery : IRequest<Result<List<ResponsibleDto>>>
    {
        public GetUniqueTagResponsiblesQuery(string projectName) => ProjectName = projectName;

        public string ProjectName { get; }
    }
}
