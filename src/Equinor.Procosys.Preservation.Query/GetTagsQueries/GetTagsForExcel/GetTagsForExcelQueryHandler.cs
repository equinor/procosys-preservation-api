using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetTagsQueries.GetTagsForExcel
{
    public class GetTagsForExcelQueryHandler : GetTagsQueryBase, IRequestHandler<GetTagsForExcelQuery, Result<IEnumerable<TagDto>>>
    {
        private readonly IReadOnlyContext _context;

        public GetTagsForExcelQueryHandler(IReadOnlyContext context) => _context = context;

        public async Task<Result<IEnumerable<TagDto>>> Handle(GetTagsForExcelQuery request, CancellationToken cancellationToken)
        {
            var queryable = CreateQueryableWithFilter(_context, request.ProjectName, request.Filter);

            queryable = AddSorting(request.Sorting, queryable);

            var orderedDtos = await queryable.ToListAsync(cancellationToken);

            if (!orderedDtos.Any())
            {
                return new SuccessResult<IEnumerable<TagDto>>(new List<TagDto>());
            }

            var tagsIds = orderedDtos.Select(dto => dto.TagId);

            // get tags again, including Requirements and PreservationPeriods. See comment in CreateQueryableWithFilter regarding Include and EF
            var tagsWithRequirements = await (from tag in _context.QuerySet<Tag>()
                        .Include(t => t.Requirements)
// ??                        .ThenInclude(r => r.PreservationPeriods)
                    where tagsIds.Contains(tag.Id)
                    select tag)
                .ToListAsync(cancellationToken);

            var requirementDefinitionIds = tagsWithRequirements.SelectMany(t => t.Requirements).Select(r => r.RequirementDefinitionId).Distinct();
            
            var reqDefs = await (from rd in _context.QuerySet<RequirementDefinition>()
                    where requirementDefinitionIds.Contains(rd.Id)
                    select new ReqDefDto
                    {
                        RequirementDefinitionId = rd.Id,
                        RequirementDefinitionTitle = rd.Title
                    }
                ).ToListAsync(cancellationToken);

            var result = CreateResult(
                orderedDtos,
                tagsWithRequirements,
                reqDefs);

            return new SuccessResult<IEnumerable<TagDto>>(result);
        }

        private IEnumerable<TagDto> CreateResult(
            List<TaqForQueryDto> orderedDtos,
            List<Tag> tagsWithRequirements,
            List<ReqDefDto> reqDefs)
        {
            var tags = orderedDtos.Select(dto =>
            {
                var tagWithRequirements = tagsWithRequirements.Single(t => t.Id == dto.TagId);
                var requirementTitles = tagWithRequirements.OrderedRequirements().Select(
                        r =>
                        {
                            var reqTypeDto = reqDefs.Single(innerDto => innerDto.RequirementDefinitionId == r.RequirementDefinitionId);
                            return reqTypeDto.RequirementDefinitionTitle;
                        })
                    .ToList();

                return new TagDto(
                    dto.GetActionStatus(),
                    dto.AreaCode,
                    dto.Calloff,
                    dto.DisciplineCode,
                    dto.IsVoided,
                    dto.ModeTitle,
                    dto.PurchaseOrderNo,
                    requirementTitles,
                    dto.ResponsibleCode,
                    dto.Status.GetDisplayValue(),
                    dto.Description,
                    dto.TagNo);
            });
            return tags;
        }

        private class ReqDefDto
        {
            public int RequirementDefinitionId { get; set; }
            public string RequirementDefinitionTitle { get; set; }
        }
    }
}
