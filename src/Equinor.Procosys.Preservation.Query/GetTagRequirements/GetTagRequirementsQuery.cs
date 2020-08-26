using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetTagRequirements
{
    public class GetTagRequirementsQuery : IRequest<Result<List<RequirementDetailsDto>>>, ITagQueryRequest
    {
        public GetTagRequirementsQuery(int tagId, bool includeVoided, bool includeAllUsages)
        {
            TagId = tagId;
            IncludeVoided = includeVoided;
            IncludeAllUsages = includeAllUsages;
        }

        public int TagId { get; }
        public bool IncludeVoided { get; }
        public bool IncludeAllUsages { get; }
    }
}
