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

namespace Equinor.ProCoSys.Preservation.Query.GetTagsCrossPlant
{
    public class GetTagsCrossPlantQueryHandler : IRequestHandler<GetTagsCrossPlantQuery, Result<List<TagDto>>>
    {
        private readonly IReadOnlyContext _context;
        private readonly IPlantCache _plantCache;
        private readonly IPlantSetter _plantSetter;

        public GetTagsCrossPlantQueryHandler(
            IReadOnlyContext context,
            IPlantCache plantCache,
            IPlantSetter plantSetter)
        {
            _context = context;
            _plantCache = plantCache;
            _plantSetter = plantSetter;
        }

        public async Task<Result<List<TagDto>>> Handle(GetTagsCrossPlantQuery request, CancellationToken cancellationToken)
        {
            _plantSetter.SetCrossPlantQuery();
            var projects = await
                (from p in _context.QuerySet<Project>()
                        .Include(t => t.Tags)
                        .ThenInclude(a => a.Attachments)
                    select p).ToListAsync(cancellationToken);
            _plantSetter.ClearCrossPlantQuery();

            var tags = new List<TagDto>();
            foreach (var project in projects)
            {
                var plant = await _plantCache.GetPlantAsync(project.Plant);
                foreach (var tag in project.Tags)
                {
                    var tagDto = new TagDto(
                        project.Plant,
                        plant.Title,
                        project.Name,
                        project.Description,
                        tag.Id,
                        tag.TagNo,
                        tag.Attachments.ToList().Count);
                    tags.Add(tagDto);
                }
            }

            var orderedTags = tags
                .OrderBy(a => a.PlantId)
                .ThenBy(a => a.ProjectName)
                .ThenBy(a => a.TagNo).ToList();
            return new SuccessResult<List<TagDto>>(orderedTags);
        }
    }
}
