using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetUniqueTagRequirementTypes
{
    public class GetUniqueTagRequirementTypesQuery : IRequest<Result<List<RequirementTypeDto>>>
    {
        public GetUniqueTagRequirementTypesQuery(string projectName) => ProjectName = projectName;

        public string ProjectName { get; }
    }
}
