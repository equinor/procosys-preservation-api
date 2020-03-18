using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetTagFunctionsHavingRequirement
{
    public class GetTagFunctionsHavingRequirementQuery : IRequest<Result<IEnumerable<TagFunctionDto>>>
    {
    }
}
