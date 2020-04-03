using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetUniqueTagAreas
{
    public class GetUniqueTagAreasQuery : IRequest<Result<List<AreaDto>>>
    {
        public GetUniqueTagAreasQuery(string projectName) => ProjectName = projectName;

        public string ProjectName { get; }
    }
}
