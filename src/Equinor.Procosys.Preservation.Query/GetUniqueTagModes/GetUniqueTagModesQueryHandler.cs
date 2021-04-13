using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetUniqueTagModes
{
    public class GetUniqueTagModesQueryHandler : IRequestHandler<GetUniqueTagModesQuery, Result<List<ModeDto>>>
    {
        private readonly IReadOnlyContext _context;

        public GetUniqueTagModesQueryHandler(IReadOnlyContext context) => _context = context;

        public async Task<Result<List<ModeDto>>> Handle(GetUniqueTagModesQuery request,
            CancellationToken cancellationToken)
        {
            var modes = await
                (from mode in _context.QuerySet<Mode>()
                    join step in _context.QuerySet<Step>()
                        on mode.Id equals step.ModeId
                    join tag in _context.QuerySet<Tag>()
                        on step.Id equals tag.StepId
                    join project in _context.QuerySet<Project>()
                        on EF.Property<int>(tag, "ProjectId") equals project.Id
                    where project.Name == request.ProjectName
                    select new ModeDto(
                        mode.Id,
                        mode.Title))
                .Distinct()
                .ToListAsync(cancellationToken);

            return new SuccessResult<List<ModeDto>>(modes);
        }
    }
}
