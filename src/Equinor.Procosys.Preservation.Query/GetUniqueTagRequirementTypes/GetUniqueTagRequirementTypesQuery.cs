using System;
using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetUniqueTagRequirementTypes
{
    public class GetUniqueTagRequirementTypesQuery : IRequest<Result<List<RequirementTypeDto>>>
    {
        public GetUniqueTagRequirementTypesQuery(string plant, string projectName)
        {
            Plant = plant ?? throw new ArgumentNullException(nameof(plant));
            ProjectName = projectName;
        }

        public string Plant { get; }
        public string ProjectName { get; }
    }
}
