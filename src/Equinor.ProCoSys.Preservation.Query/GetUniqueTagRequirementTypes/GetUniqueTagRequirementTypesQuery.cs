using System.Collections.Generic;
using Equinor.ProCoSys.Common;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetUniqueTagRequirementTypes
{
    public class GetUniqueTagRequirementTypesQuery : IRequest<Result<List<RequirementTypeDto>>>, IProjectRequest
    {
        public GetUniqueTagRequirementTypesQuery(string projectName) => ProjectName = projectName;

        public string ProjectName { get; }
    }
}
