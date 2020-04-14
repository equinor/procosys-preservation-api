using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetUniqueTagFunctions
{
    public class GetUniqueTagFunctionsQuery : IRequest<Result<List<TagFunctionCodeDto>>>, IProjectRequest
    {
        public GetUniqueTagFunctionsQuery(string projectName) => ProjectName = projectName;

        public string ProjectName { get; }
    }
}
