using System.Collections.Generic;
using Equinor.ProCoSys.Preservation.Domain;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetUniqueTagFunctions
{
    public class GetUniqueTagFunctionsQuery : IRequest<Result<List<TagFunctionCodeDto>>>, IProjectRequest
    {
        public GetUniqueTagFunctionsQuery(string projectName) => ProjectName = projectName;

        public string ProjectName { get; }
    }
}
