using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetJourneyById
{
    public class GetJourneyByIdQuery : IRequest<Result<JourneyDetailsDto>>
    {
        public GetJourneyByIdQuery(int id) => Id = id;

        public int Id { get; }
    }
}
