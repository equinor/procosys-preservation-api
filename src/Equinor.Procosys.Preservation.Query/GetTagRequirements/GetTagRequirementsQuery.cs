using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetTagRequirements
{
    public class GetTagRequirementsQuery : IRequest<Result<List<RequirementDto>>>, ITagQueryRequest
    {
        public GetTagRequirementsQuery(int tagId) => TagId = tagId;

        public int TagId { get; }
    }
}
