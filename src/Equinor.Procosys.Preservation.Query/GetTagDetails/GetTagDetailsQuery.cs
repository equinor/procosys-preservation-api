using Equinor.Procosys.Preservation.Domain;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetTagDetails
{
    public class GetTagDetailsQuery : IRequest<Result<TagDetailsDto>>, ITagRequest
    {
        public GetTagDetailsQuery(int tagId) => TagId = tagId;

        public int TagId { get; }
    }
}
