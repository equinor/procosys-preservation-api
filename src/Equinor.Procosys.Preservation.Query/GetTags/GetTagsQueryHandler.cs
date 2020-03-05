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

namespace Equinor.Procosys.Preservation.Query.GetTags
{
    public class GetTagsQueryHandler : IRequestHandler<GetTagsQuery, Result<IEnumerable<TagDto>>>
    {
        private readonly IReadOnlyContext _context;
        private readonly ITimeService _timeService;

        public GetTagsQueryHandler(
            IReadOnlyContext context,
            ITimeService timeService)
        {
            _timeService = timeService;
            _context = context;
        }

        public async Task<Result<IEnumerable<TagDto>>> Handle(GetTagsQuery request, CancellationToken cancellationToken)
        {
            var queryable = from tag in _context.QuerySet<Tag>().Include(t => t.Requirements).ThenInclude(r => r.PreservationPeriods)
                join step in _context.QuerySet<Step>() on tag.StepId equals step.Id
                join journey in _context.QuerySet<Journey>().Include(j => j.Steps) on EF.Property<int>(step, "JourneyId") equals journey.Id
                join mode in _context.QuerySet<Mode>() on step.ModeId equals mode.Id
                join responsible in _context.QuerySet<Responsible>() on step.ResponsibleId equals responsible.Id
                join project in _context.QuerySet<Project>() on EF.Property<int>(tag, "ProjectId") equals project.Id
                //from req in _context.QuerySet<Requirement>()
                //join reqDef in _context.QuerySet<RequirementDefinition>() on req.RequirementDefinitionId equals reqDef.Id
                //join reqType in _context.QuerySet<RequirementType>() on EF.Property<int>(reqDef, "RequirementTypeId") equals reqType.Id
                where project.Name == request.Filter.ProjectName &&
                      (!request.Filter.PreservationStatus.HasValue || tag.Status == request.Filter.PreservationStatus.Value) &&
                      (string.IsNullOrEmpty(request.Filter.TagNo) || tag.TagNo.Contains(request.Filter.TagNo)) &&
                      (string.IsNullOrEmpty(request.Filter.McPkgNo) || tag.McPkgNo.Contains(request.Filter.McPkgNo)) &&
                      (!request.Filter.ResponsibleIds.Any() || request.Filter.ResponsibleIds.Contains(responsible.Id)) &&
                      //(!request.Filter.RequirementTypeIds.Any() || request.Filter.RequirementTypeIds.Contains(reqType.Id)) &&
                      (!request.Filter.ModeIds.Any() || request.Filter.ModeIds.Contains(mode.Id)) &&
                      (!request.Filter.StepIds.Any() || request.Filter.StepIds.Contains(step.Id))
                select new Dto
                {
                    Tag = tag,
                    Responsible = responsible,
                    Mode = mode,
                    Journey = journey
                };

            queryable = AddSorting(request.Sorting, queryable);
            
            var orderedDtos = await queryable
                .Skip(request.Paging.Page*request.Paging.Size)
                .Take(request.Paging.Size)
                .ToListAsync(cancellationToken);

            if (!orderedDtos.Any())
            {
                return new SuccessResult<IEnumerable<TagDto>>(new List<TagDto>());
            }

            var reqDefIds = orderedDtos.Select(dto => dto.Tag).SelectMany(t => t.Requirements)
                .Select(r => r.RequirementDefinitionId).Distinct();

            var reqTypeDtos = await (from rd in _context.QuerySet<RequirementDefinition>()
                    join rt in _context.QuerySet<RequirementType>() on EF.Property<int>(rd, "RequirementTypeId") equals rt.Id
                    where reqDefIds.Contains(rd.Id)
                    select new ReqTypeDto
                    {
                        RequirementDefinitionId = rd.Id,
                        RequirementTypeCode = rt.Code
                    }
                ).ToListAsync(cancellationToken);

            var now = _timeService.GetCurrentTimeUtc();

            var tagDtos = orderedDtos.Select(outerDto =>
            {
                var tag = outerDto.Tag;
                var requirementDtos = tag.OrderedRequirements().Select(
                        r =>
                        {
                            var reqTypeDto =
                                reqTypeDtos.Single(innerDto => innerDto.RequirementDefinitionId == r.RequirementDefinitionId);
                            return new RequirementDto(
                                r.Id,
                                reqTypeDto.RequirementTypeCode,
                                r.NextDueTimeUtc,
                                r.GetNextDueInWeeks(now),
                                r.IsReadyAndDueToBePreserved(now));
                        })
                    .ToList();

                return new TagDto(tag.Id,
                    tag.AreaCode,
                    tag.Calloff,
                    tag.CommPkgNo,
                    tag.DisciplineCode,
                    tag.IsVoided,
                    tag.McPkgNo,
                    outerDto.Mode.Title,
                    tag.IsReadyToBePreserved(now),
                    tag.IsReadyToBeTransferred(outerDto.Journey),
                    tag.PurchaseOrderNo,
                    tag.Remark,
                    requirementDtos,
                    outerDto.Responsible.Code,
                    tag.Status,
                    tag.TagFunctionCode,
                    tag.Description,
                    tag.TagNo,
                    tag.TagType);
            });
            return new SuccessResult<IEnumerable<TagDto>>(tagDtos);
        }

        private static IQueryable<Dto> AddSorting(Sorting sorting, IQueryable<Dto> queryable)
        {
            switch (sorting.SortingDirection)
            {
                case SortingDirection.Asc:
                    switch (sorting.SortingColumn)
                    {
                        //case SortingColumn.NextDue:
                        //    queryable = queryable.OrderBy(dto => dto.Tag.NextDueTimeUtc);
                        //    break;
                        case SortingColumn.Status:
                            queryable = queryable.OrderBy(dto => dto.Tag.Status);
                            break;
                        case SortingColumn.TagNo:
                            queryable = queryable.OrderBy(dto => dto.Tag.TagNo);
                            break;
                        case SortingColumn.Description:
                            queryable = queryable.OrderBy(dto => dto.Tag.Description);
                            break;
                        case SortingColumn.Responsible:
                            queryable = queryable.OrderBy(dto => dto.Responsible.Code);
                            break;
                        case SortingColumn.Mode:
                            queryable = queryable.OrderBy(dto => dto.Mode.Title);
                            break;
                        case SortingColumn.PO:
                            queryable = queryable.OrderBy(dto => dto.Tag.Calloff);
                            break;
                        case SortingColumn.Area:
                            queryable = queryable.OrderBy(dto => dto.Tag.AreaCode);
                            break;
                        case SortingColumn.Discipline:
                            queryable = queryable.OrderBy(dto => dto.Tag.DisciplineCode);
                            break;
                        default:
                            queryable = queryable.OrderBy(dto => dto.Tag.Id);
                            break;
                    }

                    break;
                case SortingDirection.Desc:
                    switch (sorting.SortingColumn)
                    {
                        //case SortingColumn.NextDue:
                        //    queryable = queryable.OrderByDescending(dto => dto.Tag.NextDueTimeUtc);
                        //    break;
                        case SortingColumn.Status:
                            queryable = queryable.OrderByDescending(dto => dto.Tag.Status);
                            break;
                        case SortingColumn.TagNo:
                            queryable = queryable.OrderByDescending(dto => dto.Tag.TagNo);
                            break;
                        case SortingColumn.Description:
                            queryable = queryable.OrderByDescending(dto => dto.Tag.Description);
                            break;
                        case SortingColumn.Responsible:
                            queryable = queryable.OrderByDescending(dto => dto.Responsible.Code);
                            break;
                        case SortingColumn.Mode:
                            queryable = queryable.OrderByDescending(dto => dto.Mode.Title);
                            break;
                        case SortingColumn.PO:
                            queryable = queryable.OrderByDescending(dto => dto.Tag.Calloff);
                            break;
                        case SortingColumn.Area:
                            queryable = queryable.OrderByDescending(dto => dto.Tag.AreaCode);
                            break;
                        case SortingColumn.Discipline:
                            queryable = queryable.OrderByDescending(dto => dto.Tag.DisciplineCode);
                            break;
                        default:
                            queryable = queryable.OrderByDescending(dto => dto.Tag.Id);
                            break;
                    }

                    break;
            }

            return queryable;
        }

        private class Dto
        {
            public Tag Tag { get; set; }
            public Responsible Responsible { get; set; }
            public Journey Journey { get; set; }
            public Mode Mode { get; set; }
        }

        private class ReqTypeDto
        {
            public int RequirementDefinitionId { get; set; }
            public string RequirementTypeCode { get; set; }
        }
    }
}
