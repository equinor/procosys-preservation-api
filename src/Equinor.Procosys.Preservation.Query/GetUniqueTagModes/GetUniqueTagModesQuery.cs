using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetUniqueTagModes
{
    public class GetUniqueTagModesQuery : IRequest<Result<List<ModeDto>>>
    {
        public GetUniqueTagModesQuery(string projectName) => ProjectName = projectName;

        public string ProjectName { get; }
    }
}
