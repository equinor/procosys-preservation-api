using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetUniqueTagAreas
{
    public class GetUniqueTagAreasQuery : IRequest<Result<List<AreaDto>>>, IProjectRequest
    {
        public GetUniqueTagAreasQuery(string projectName) => ProjectName = projectName;

        public string ProjectName { get; }
    }
}
