using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetUniqueTagJourneys
{
    public class GetUniqueTagJourneysQuery : IRequest<Result<List<JourneyDto>>>, IProjectRequest
    {
        public GetUniqueTagJourneysQuery(string projectName) => ProjectName = projectName;

        public string ProjectName { get; }
    }
}
