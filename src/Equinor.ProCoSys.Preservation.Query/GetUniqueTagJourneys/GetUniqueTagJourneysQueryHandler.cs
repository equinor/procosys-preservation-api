using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetUniqueTagJourneys
{
    public class GetUniqueTagJourneysQueryHandler : IRequestHandler<GetUniqueTagJourneysQuery, Result<List<JourneyDto>>>
    {
        private readonly IReadOnlyContext _context;

        public GetUniqueTagJourneysQueryHandler(IReadOnlyContext context) => _context = context;

        public async Task<Result<List<JourneyDto>>> Handle(GetUniqueTagJourneysQuery request,
            CancellationToken cancellationToken)
        {
            var journeys = await
                (from journey in _context.QuerySet<Journey>()
                    join step in _context.QuerySet<Step>()
                        on journey.Id equals EF.Property<int>(step, "JourneyId")
                    join tag in _context.QuerySet<Tag>()
                        on step.Id equals tag.StepId
                    join project in _context.QuerySet<Project>()
                        on EF.Property<int>(tag, "ProjectId") equals project.Id
                    where project.Name == request.ProjectName
                    select new JourneyDto(
                        journey.Id,
                        journey.Title))
                .Distinct()
                .ToListAsync(cancellationToken);

            return new SuccessResult<List<JourneyDto>>(journeys);
        }
    }
}
