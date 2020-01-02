using MediatR;

namespace Equinor.Procosys.Preservation.Query.JourneyAggregate
{
    public class GetJourneyByIdQuery : IRequest<JourneyDto>
    {
        public GetJourneyByIdQuery(int id) => Id = id;

        public int Id { get; }
    }
}
