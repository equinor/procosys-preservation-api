using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.JourneyAggregate
{
    public class GetJourneyByIdQuery : IRequest<Result<JourneyDto>>
    {
        public GetJourneyByIdQuery(int id) => Id = id;

        public int Id { get; }
    }
}
