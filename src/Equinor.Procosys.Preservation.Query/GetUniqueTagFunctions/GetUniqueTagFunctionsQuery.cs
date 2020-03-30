using System;
using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetUniqueTagFunctions
{
    public class GetUniqueTagFunctionsQuery : IRequest<Result<List<TagFunctionCodeDto>>>
    {
        public GetUniqueTagFunctionsQuery(string plant, string projectName)
        {
            Plant = plant ?? throw new ArgumentNullException(nameof(plant));
            ProjectName = projectName;
        }

        public string Plant { get; }
        public string ProjectName { get; }
    }
}
