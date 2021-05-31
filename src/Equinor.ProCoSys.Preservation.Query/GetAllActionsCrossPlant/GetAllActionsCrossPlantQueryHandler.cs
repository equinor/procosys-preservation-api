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

namespace Equinor.ProCoSys.Preservation.Query.GetAllActionsCrossPlant
{
    public class GetAllActionsCrossPlantQueryHandler : IRequestHandler<GetAllActionsCrossPlantQuery, Result<List<ActionDto>>>
    {
        private readonly IReadOnlyContext _context;
        private readonly IPlantCache _plantCache;
        private readonly IPlantSetter _plantSetter;

        public GetAllActionsCrossPlantQueryHandler(
            IReadOnlyContext context,
            IPlantCache plantCache,
            IPlantSetter plantSetter)
        {
            _context = context;
            _plantCache = plantCache;
            _plantSetter = plantSetter;
        }

        public async Task<Result<List<ActionDto>>> Handle(GetAllActionsCrossPlantQuery request, CancellationToken cancellationToken)
        {
            // Get all tags with all actions
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
                var plant = await _plantCache.GetPlantAsync(project.Plant);
                foreach (var tag in project.Tags.Where(t => t.Actions.Count > 0))
                {
                    foreach (var action in tag.Actions)
                    {
                        var actionDto = new ActionDto(
                            project.Plant,
                            plant.Title,
                            project.Name,
                            project.Description,
                            tag.Id,
                            tag.TagNo,
                            action.Id,
                            action.Title,
                            action.IsOverDue(),
                            action.DueTimeUtc,
                            action.IsClosed,
                            action.Attachments.ToList().Count);
                        actions.Add(actionDto);
                    }
                }
            }

            var orderedActions = actions
                .OrderBy(a => a.Plant)
                .ThenBy(a => a.ProjectName)
                .ThenBy(a => a.TagNo)
                .ThenBy(a => a.Title).ToList();
            return new SuccessResult<List<ActionDto>>(orderedActions);
        }
    }
}
