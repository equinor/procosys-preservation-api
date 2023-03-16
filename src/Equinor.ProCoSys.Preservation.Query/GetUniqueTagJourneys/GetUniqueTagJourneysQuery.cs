using System.Collections.Generic;
using Equinor.ProCoSys.Common;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetUniqueTagJourneys
{
    public class GetUniqueTagJourneysQuery : IRequest<Result<List<JourneyDto>>>, IProjectRequest
    {
        public GetUniqueTagJourneysQuery(string projectName) => ProjectName = projectName;

        public string ProjectName { get; }
    }
}
