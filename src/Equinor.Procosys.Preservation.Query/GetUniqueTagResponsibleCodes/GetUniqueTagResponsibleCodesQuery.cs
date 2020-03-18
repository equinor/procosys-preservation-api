using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetUniqueTagResponsibleCodes
{
    public class GetUniqueTagResponsibleCodesQuery : IRequest<Result<List<ResponsibleDto>>>
    {
        public GetUniqueTagResponsibleCodesQuery(string projectName) => ProjectName = projectName;

        public string ProjectName { get; }
    }
}
