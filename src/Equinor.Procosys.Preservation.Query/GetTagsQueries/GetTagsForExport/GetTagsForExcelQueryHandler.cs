using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetTagsQueries.GetTagsForExport
{
    public class GetTagsForExportQueryHandler : GetTagsQueryBase, IRequestHandler<GetTagsForExportQuery, Result<ExportDto>>
    {
        private readonly IReadOnlyContext _context;
        private readonly IPlantProvider _plantProvider;

        // todo unit test
        public GetTagsForExportQueryHandler(IReadOnlyContext context, IPlantProvider plantProvider)
        {
            _context = context;
            _plantProvider = plantProvider;
        }

        public async Task<Result<ExportDto>> Handle(GetTagsForExportQuery request, CancellationToken cancellationToken)
        {
            var queryable = CreateQueryableWithFilter(_context, request.ProjectName, request.Filter);

            queryable = AddSorting(request.Sorting, queryable);

            var orderedDtos = await queryable.ToListAsync(cancellationToken);

            var usedFilterDto = await CreateUsedFilterDtoAsync(request.ProjectName, request.Filter);
            if (!orderedDtos.Any())
            {
                return new SuccessResult<ExportDto>(new ExportDto(null, usedFilterDto));
            }

            var tagsIds = orderedDtos.Select(dto => dto.TagId);

            // get tags again, including Requirements. See comment in CreateQueryableWithFilter regarding Include and EF
            var tagsWithRequirements = await (from tag in _context.QuerySet<Tag>()
                        .Include(t => t.Requirements)
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

            var tags = CreateTagDtos(
                orderedDtos,
                tagsWithRequirements,
                reqDefs);

            return new SuccessResult<ExportDto>(new ExportDto(tags, usedFilterDto));
        }

        private async Task<UsedFilterDto> CreateUsedFilterDtoAsync(string projectName, Filter filter)
        {
            var projectDescription = await GetProjectDescriptionAsync(projectName);
            var requirementTypeTitles = await GetRequirementTypeTitlesAsync(filter.RequirementTypeIds);
            var responsibleCodes = await GetResponsibleCodesAsync(filter.ResponsibleIds);
            var modeTitles = await GetModeTitlesAsync(filter.ModeIds);
            var journeyTitles = await GetJourneyTitlesAsync(filter.JourneyIds);

            return new UsedFilterDto(
                filter.ActionStatus.HasValue ? filter.ActionStatus.Value.ToString() : string.Empty,
                filter.AreaCodes,
                filter.CallOffStartsWith,
                filter.CommPkgNoStartsWith,
                filter.DisciplineCodes,
                filter.DueFilters.Select(v => v.ToString()), 
                journeyTitles,
                filter.McPkgNoStartsWith,
                modeTitles,
                filter.PreservationStatus.HasValue ? filter.PreservationStatus.Value.ToString() : string.Empty,
                projectDescription,
                _plantProvider.Plant,
                projectName,
                filter.PurchaseOrderNoStartsWith,
                requirementTypeTitles,
                responsibleCodes,
                filter.StorageAreaStartsWith,
                filter.TagFunctionCodes,
                filter.TagNoStartsWith,
                filter.VoidedFilter.ToString());
        }

        private async Task<string> GetProjectDescriptionAsync(string projectName) 
            => await (from p in _context.QuerySet<Project>()
                where p.Name == projectName
                select p.Description).SingleOrDefaultAsync();

        private async Task<List<string>> GetJourneyTitlesAsync(IList<int> journeyIds)
        {
            if (!journeyIds.Any())
            {
                return new List<string>();
            }

            return await (from j in _context.QuerySet<Journey>()
                where journeyIds.Contains(j.Id)
                select j.Title).ToListAsync();
        }

        private async Task<List<string>> GetModeTitlesAsync(IList<int> modeIds)
        {
            if (!modeIds.Any())
            {
                return new List<string>();
            }

            return await (from m in _context.QuerySet<Mode>()
                where modeIds.Contains(m.Id)
                select m.Title).ToListAsync();
        }

        private async Task<List<string>> GetResponsibleCodesAsync(IList<int> responsibleIds)
        {
            if (!responsibleIds.Any())
            {
                return new List<string>();
            }

            return await (from r in _context.QuerySet<Responsible>()
                where responsibleIds.Contains(r.Id)
                select r.Code).ToListAsync();
        }

        private async Task<List<string>> GetRequirementTypeTitlesAsync(IList<int> requirementTypeIds)
        {
            if (!requirementTypeIds.Any())
            {
                return new List<string>();
            }

            return await (from r in _context.QuerySet<RequirementType>()
                where requirementTypeIds.Contains(r.Id)
                select r.Title).ToListAsync();
        }

        private IEnumerable<ExportTagDto> CreateTagDtos(
            List<TaqForQueryDto> orderedDtos,
            List<Tag> tagsWithRequirements,
            List<ReqDefDto> reqDefs)
        {
            var tags = orderedDtos.Select(dto =>
            {
                var tagWithRequirements = tagsWithRequirements.Single(t => t.Id == dto.TagId);
                var orderedRequirements = tagWithRequirements.OrderedRequirements().ToList();
                var requirementTitles = orderedRequirements.Select(
                        r =>
                        {
                            var reqTypeDto = reqDefs.Single(innerDto => innerDto.RequirementDefinitionId == r.RequirementDefinitionId);
                            return reqTypeDto.RequirementDefinitionTitle;
                        })
                    .ToList();

                int? nextDueWeeks = null;
                var nextDueAsYearAndWeek = string.Empty;
                
                var firstUpcomingRequirement = orderedRequirements.FirstOrDefault();
                if (firstUpcomingRequirement != null)
                {
                    nextDueWeeks = firstUpcomingRequirement.GetNextDueInWeeks();
                    nextDueAsYearAndWeek = firstUpcomingRequirement.NextDueTimeUtc?.FormatAsYearAndWeekString();
                }

                return new ExportTagDto(
                    dto.GetActionStatus().GetDisplayValue(),
                    dto.AreaCode,
                    dto.DisciplineCode,
                    dto.IsVoided,
                    dto.ModeTitle,
                    nextDueAsYearAndWeek,
                    nextDueWeeks,
                    string.IsNullOrEmpty(dto.CalloffNo) ? dto.PurchaseOrderNo : $"{dto.PurchaseOrderNo}/{dto.CalloffNo}",
                    string.Join(",", requirementTitles),
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
