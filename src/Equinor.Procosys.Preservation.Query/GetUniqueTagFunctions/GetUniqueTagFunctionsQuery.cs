using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetUniqueTagFunctions
{
    public class GetUniqueTagFunctionsQuery : IRequest<Result<List<TagFunctionCodeDto>>>
    {
        public GetUniqueTagFunctionsQuery(string projectName) => ProjectName = projectName;

        public string ProjectName { get; }
    }
}
