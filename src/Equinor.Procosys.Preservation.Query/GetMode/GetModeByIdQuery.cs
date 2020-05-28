using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetMode
{
    public class GetModeByIdQuery : IRequest<Result<ModeDto>>
    {
        public GetModeByIdQuery(int id) => Id = id;

        public int Id { get; }
    }
}
