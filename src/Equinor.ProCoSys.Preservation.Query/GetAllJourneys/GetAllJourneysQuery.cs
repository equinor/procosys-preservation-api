using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetAllJourneys
{
    public class GetAllJourneysQuery(bool includeVoided, string projectName) : IRequest<Result<IEnumerable<JourneyDto>>>
    {
        public bool IncludeVoided { get; } = includeVoided;
        public string ProjectName { get; } = projectName;
    }
}
