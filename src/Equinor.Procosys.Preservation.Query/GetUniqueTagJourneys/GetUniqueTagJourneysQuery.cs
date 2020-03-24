using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetUniqueTagJourneys
{
    public class GetUniqueTagJourneysQuery : IRequest<Result<List<JourneyDto>>>
    {
        public GetUniqueTagJourneysQuery(string projectName) => ProjectName = projectName;

        public string ProjectName { get; }
    }
}
