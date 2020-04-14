using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetUniqueTagRequirementTypes
{
    public class GetUniqueTagRequirementTypesQuery : IRequest<Result<List<RequirementTypeDto>>>, IProjectRequest
    {
        public GetUniqueTagRequirementTypesQuery(string projectName) => ProjectName = projectName;

        public string ProjectName { get; }
    }
}
