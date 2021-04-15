using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetJourneyById
{
    public class GetJourneyByIdQuery : IRequest<Result<JourneyDetailsDto>>
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
