using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetTagDetails
{
    public class GetTagDetailsQuery : IRequest<Result<TagDetailsDto>>, ITagQueryRequest
    {
        public GetTagDetailsQuery(int tagId) => TagId = tagId;

        public int TagId { get; }
    }
}
