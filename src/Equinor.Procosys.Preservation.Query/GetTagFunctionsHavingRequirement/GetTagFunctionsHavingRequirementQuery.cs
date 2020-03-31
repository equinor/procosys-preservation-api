using System;
using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetTagFunctionsHavingRequirement
{
    public class GetTagFunctionsHavingRequirementQuery : IRequest<Result<IEnumerable<TagFunctionDto>>>
    {
        public GetTagFunctionsHavingRequirementQuery(string plant)
            => Plant = plant ?? throw new ArgumentNullException(nameof(plant));

        public string Plant { get; }
    }
}
