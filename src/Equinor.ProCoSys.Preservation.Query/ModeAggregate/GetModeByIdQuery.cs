using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.ModeAggregate
{
    public class GetModeByIdQuery : IRequest<Result<ModeDto>>
    {
        public GetModeByIdQuery(int id) => Id = id;

        public int Id { get; }
    }
}
