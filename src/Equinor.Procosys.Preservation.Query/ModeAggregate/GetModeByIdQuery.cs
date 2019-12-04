using MediatR;

namespace Equinor.Procosys.Preservation.Query.ModeAggregate
{
    public class GetModeByIdQuery : IRequest<ModeDto>
    {
        public GetModeByIdQuery(int id)
        {
            Id = id;
        }

        public int Id { get; }
    }
}
