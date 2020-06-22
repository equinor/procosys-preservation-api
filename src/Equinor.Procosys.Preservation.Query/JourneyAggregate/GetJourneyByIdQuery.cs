using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.JourneyAggregate
{
    public class GetJourneyByIdQuery : IRequest<Result<JourneyDto>>
    {
        public GetJourneyByIdQuery(int id, bool includeVoided)
        {
            Id = id;
            IncludeVoided = includeVoided;
        }

        public int Id { get; }
        public bool IncludeVoided { get; }
    }
}
