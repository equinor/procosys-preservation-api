using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.MainApi.Plant;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetActionsCrossPlant
{
    public class GetActionsCrossPlantQueryHandler : IRequestHandler<GetActionsCrossPlantQuery, Result<List<ActionDto>>>
    {
        private readonly IReadOnlyContext _context;
        private readonly IPlantCache _plantCache;
        private readonly IPlantSetter _plantSetter;

        public GetActionsCrossPlantQueryHandler(
            IReadOnlyContext context,
            IPlantCache plantCache,
            IPlantSetter plantSetter)
        {
            _context = context;
            _plantCache = plantCache;
            _plantSetter = plantSetter;
        }

        public async Task<Result<List<ActionDto>>> Handle(GetActionsCrossPlantQuery request, CancellationToken cancellationToken)
        {
            _plantSetter.SetCrossPlantQuery();
            var projects = await
                (from p in _context.QuerySet<Project>()
                        .Include(t => t.Tags)
                        .ThenInclude(t => t.Actions)
                        .ThenInclude(a => a.Attachments)
                    select p).ToListAsync(cancellationToken);
            _plantSetter.ClearCrossPlantQuery();

            var actions = new List<ActionDto>();
            foreach (var project in projects)
            {
                var plantTitle = await _plantCache.GetPlantTitleAsync(project.Plant);
                foreach (var tag in project.Tags.Where(t => t.Actions.Count > 0))
                {
                    foreach (var action in tag.Actions)
                    {
                        var actionDto = new ActionDto(
                            project.Plant,
                            plantTitle,
                            project.Name,
                            project.Description,
                            project.IsClosed,
                            tag.Id,
                            tag.TagNo,
                            action.Id,
                            action.Title,
                            action.Description,
                            action.IsOverDue(),
                            action.DueTimeUtc,
                            action.IsClosed,
                            action.Attachments.ToList().Count);
                        actions.Add(actionDto);
                    }
                }
            }

            var orderedActions = actions
                .OrderBy(a => a.PlantId)
                .ThenBy(a => a.ProjectName)
                .ThenBy(a => a.TagNo)
                .ThenBy(a => a.Title)
                .AsEnumerable();

            if (request.Max > 0)
            {
                orderedActions = orderedActions.Take(request.Max);
            }
            return new SuccessResult<List<ActionDto>>(orderedActions.ToList());
        }
    }
}
