using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetTagRequirements
{
    public class GetTagRequirementsQuery : IRequest<Result<List<RequirementDto>>>, ITagRequest
    {
        public GetTagRequirementsQuery(int tagId) => TagId = tagId;

        public int TagId { get; }
    }
}
