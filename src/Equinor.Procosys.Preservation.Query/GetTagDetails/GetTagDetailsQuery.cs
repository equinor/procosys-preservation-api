using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetTagDetails
{
    public class GetTagDetailsQuery : IRequest<Result<TagDetailsDto>>
    {
        public GetTagDetailsQuery(int id)
        {
            Id = id;
        }

        public int Id { get; }
    }
}
