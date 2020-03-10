using System;
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
using RequirementType = Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate.RequirementType;

namespace Equinor.Procosys.Preservation.Query.GetTags
{
    public class GetTagsQueryHandler : IRequestHandler<GetTagsQuery, Result<TagsResult>>
    {
        private readonly IReadOnlyContext _context;

        public GetTagsQueryHandler(
            IReadOnlyContext context) =>
            _context = context;

        public async Task<Result<TagsResult>> Handle(GetTagsQuery request, CancellationToken cancellationToken)
        {
            // No .Include() here. EF do not support .Include together with selecting a projection (dto).
            // If the select-statement select tag so queryable has been of type IQueryable<Tag>, .Include(t => t.Requirements) work fine
            var queryable = from tag in _context.QuerySet<Tag>()
                join project in _context.QuerySet<Project>() on EF.Property<int>(tag, "ProjectId") equals project.Id
                join step in _context.QuerySet<Step>() on tag.StepId equals step.Id
                join journey in _context.QuerySet<Journey>() on EF.Property<int>(step, "JourneyId") equals journey.Id
                join mode in _context.QuerySet<Mode>() on step.ModeId equals mode.Id
                join responsible in _context.QuerySet<Responsible>() on step.ResponsibleId equals responsible.Id
                let reqTypeFiltered = (from req in _context.QuerySet<Requirement>()
                    join reqDef in _context.QuerySet<RequirementDefinition>() on req.RequirementDefinitionId equals reqDef.Id
                    join reqType in _context.QuerySet<RequirementType>() on EF.Property<int>(reqDef, "RequirementTypeId") equals reqType.Id
                    where EF.Property<int>(req, "TagId") == tag.Id &&
                          request.Filter.RequirementTypeIds.Contains(reqType.Id)
                    select reqType.Id).Any()
                where project.Name == request.Filter.ProjectName &&
                      (!request.Filter.PreservationStatus.HasValue || tag.Status == request.Filter.PreservationStatus.Value) &&
                      (string.IsNullOrEmpty(request.Filter.TagNoStartsWith) || tag.TagNo.StartsWith(request.Filter.TagNoStartsWith)) &&
                      (string.IsNullOrEmpty(request.Filter.CommPkgNoStartsWith) || tag.CommPkgNo.StartsWith(request.Filter.CommPkgNoStartsWith)) &&
                      (string.IsNullOrEmpty(request.Filter.McPkgNoStartsWith) || tag.McPkgNo.StartsWith(request.Filter.McPkgNoStartsWith)) &&
                      (string.IsNullOrEmpty(request.Filter.PurchaseOrderNoStartsWith) || tag.PurchaseOrderNo.StartsWith(request.Filter.PurchaseOrderNoStartsWith)) &&
                      (string.IsNullOrEmpty(request.Filter.CallOffStartsWith) || tag.Calloff.StartsWith(request.Filter.CallOffStartsWith)) &&
                      (!request.Filter.RequirementTypeIds.Any() || reqTypeFiltered) &&
                      (!request.Filter.TagFunctionCodes.Any() || request.Filter.TagFunctionCodes.Contains(tag.TagFunctionCode)) &&
                      (!request.Filter.ResponsibleIds.Any() || request.Filter.ResponsibleIds.Contains(responsible.Id)) &&
                      (!request.Filter.JourneyIds.Any() || request.Filter.JourneyIds.Contains(journey.Id)) &&
                      (!request.Filter.ModeIds.Any() || request.Filter.ModeIds.Contains(mode.Id)) &&
                      (!request.Filter.StepIds.Any() || request.Filter.StepIds.Contains(step.Id))
                select new Dto
                {
                    TagId = tag.Id,
                    AreaCode = tag.AreaCode,
                    Calloff = tag.Calloff,
                    CommPkgNo = tag.CommPkgNo,
                    Description = tag.Description,
                    DisciplineCode = tag.DisciplineCode,
                    IsVoided = tag.IsVoided,
                    McPkgNo = tag.McPkgNo,
                    NextDueTimeUtc = tag.NextDueTimeUtc,
                    PurchaseOrderNo = tag.PurchaseOrderNo,
                    Status = tag.Status,
                    TagFunctionCode = tag.TagFunctionCode,
                    TagNo = tag.TagNo,
                    ResponsibleCode = responsible.Code,
                    ModeTitle = mode.Title
                };

            queryable = AddDueFilter(request.Filter.DueFilters.ToList(), queryable);

            var result = new TagsResult();

            // todo spawn 2 threads: CountAsync in one and filtered/paged query in another
            result.MaxAvailable = await queryable.CountAsync(cancellationToken);

            queryable = AddSorting(request.Sorting, queryable);
            queryable = AddPaging(request.Paging, queryable);

            var orderedDtos = await queryable.ToListAsync(cancellationToken);

            if (!orderedDtos.Any())
            {
                return new SuccessResult<TagsResult>(new TagsResult());
            }

            var tagsIds = orderedDtos.Select(t => t.TagId);

            var tagsWithRequirements = await (from tag in _context.QuerySet<Tag>().Include(t => t.Requirements).ThenInclude(r => r.PreservationPeriods)
                    where tagsIds.Contains(tag.Id)
                    select tag)
                .ToListAsync(cancellationToken);

            var requirementDefinitionIds = tagsWithRequirements.SelectMany(t => t.Requirements)
                .Select(r => r.RequirementDefinitionId).Distinct();

            var reqTypeDtos = await (from rd in _context.QuerySet<RequirementDefinition>()
                    join rt in _context.QuerySet<RequirementType>() on EF.Property<int>(rd, "RequirementTypeId") equals rt.Id
                    where requirementDefinitionIds.Contains(rd.Id)
                    select new ReqTypeDto
                    {
                        RequirementDefinitionId = rd.Id,
                        RequirementTypeCode = rt.Code
                    }
                ).ToListAsync(cancellationToken);

            result.Tags = orderedDtos.Select(tagDto =>
            {
                var tagWithRequirement = tagsWithRequirements.Single(t => t.Id == tagDto.TagId);

                var requirementDtos = tagWithRequirement.OrderedRequirements().Select(
                        r =>
                        {
                            var reqTypeDto =
                                reqTypeDtos.Single(innerDto => innerDto.RequirementDefinitionId == r.RequirementDefinitionId);
                            return new RequirementDto(
                                r.Id,
                                reqTypeDto.RequirementTypeCode,
                                r.NextDueTimeUtc,
                                r.GetNextDueInWeeks(),
                                r.IsReadyAndDueToBePreserved());
                        })
                    .ToList();

                return new TagDto(tagDto.TagId,
                    tagDto.AreaCode,
                    tagDto.Calloff,
                    tagDto.CommPkgNo,
                    tagDto.DisciplineCode,
                    tagDto.IsVoided,
                    tagDto.McPkgNo,
                    tagDto.ModeTitle,
                    tagDto.PurchaseOrderNo,
                    requirementDtos,
                    tagDto.ResponsibleCode,
                    tagDto.Status,
                    tagDto.TagFunctionCode,
                    tagDto.Description,
                    tagDto.TagNo);
            });
            return new SuccessResult<TagsResult>(result);
        }

        private IQueryable<Dto> AddDueFilter(List<DueFilterType> dueFilters, IQueryable<Dto> queryable)
        {
            if (!dueFilters.Any())
            {
                return queryable;
            }

            // todo
            return queryable;
        }

        private static IQueryable<Dto> AddPaging(Paging paging, IQueryable<Dto> queryable)
        {
            queryable = queryable
                .Skip(paging.Page * paging.Size)
                .Take(paging.Size);
            return queryable;
        }

        private static IQueryable<Dto> AddSorting(Sorting sorting, IQueryable<Dto> queryable)
        {
            switch (sorting.SortingDirection)
            {
                case SortingDirection.Asc:
                    switch (sorting.SortingColumn)
                    {
                        case SortingColumn.Due:
                            queryable = queryable.OrderBy(dto => dto.NextDueTimeUtc);
                            break;
                        case SortingColumn.Status:
                            queryable = queryable.OrderBy(dto => dto.Status);
                            break;
                        case SortingColumn.TagNo:
                            queryable = queryable.OrderBy(dto => dto.TagNo);
                            break;
                        case SortingColumn.Description:
                            queryable = queryable.OrderBy(dto => dto.Description);
                            break;
                        case SortingColumn.Responsible:
                            queryable = queryable.OrderBy(dto => dto.ResponsibleCode);
                            break;
                        case SortingColumn.Mode:
                            queryable = queryable.OrderBy(dto => dto.ModeTitle);
                            break;
                        case SortingColumn.PO:
                            queryable = queryable.OrderBy(dto => dto.Calloff);
                            break;
                        case SortingColumn.Area:
                            queryable = queryable.OrderBy(dto => dto.AreaCode);
                            break;
                        case SortingColumn.Discipline:
                            queryable = queryable.OrderBy(dto => dto.DisciplineCode);
                            break;
                        default:
                            queryable = queryable.OrderBy(dto => dto.TagId);
                            break;
                    }

                    break;
                case SortingDirection.Desc:
                    switch (sorting.SortingColumn)
                    {
                        case SortingColumn.Due:
                            queryable = queryable.OrderByDescending(dto => dto.NextDueTimeUtc);
                            break;
                        case SortingColumn.Status:
                            queryable = queryable.OrderByDescending(dto => dto.Status);
                            break;
                        case SortingColumn.TagNo:
                            queryable = queryable.OrderByDescending(dto => dto.TagNo);
                            break;
                        case SortingColumn.Description:
                            queryable = queryable.OrderByDescending(dto => dto.Description);
                            break;
                        case SortingColumn.Responsible:
                            queryable = queryable.OrderByDescending(dto => dto.ResponsibleCode);
                            break;
                        case SortingColumn.Mode:
                            queryable = queryable.OrderByDescending(dto => dto.ModeTitle);
                            break;
                        case SortingColumn.PO:
                            queryable = queryable.OrderByDescending(dto => dto.Calloff);
                            break;
                        case SortingColumn.Area:
                            queryable = queryable.OrderByDescending(dto => dto.AreaCode);
                            break;
                        case SortingColumn.Discipline:
                            queryable = queryable.OrderByDescending(dto => dto.DisciplineCode);
                            break;
                        default:
                            queryable = queryable.OrderByDescending(dto => dto.TagId);
                            break;
                    }

                    break;
            }

            return queryable;
        }

        private class Dto
        {
            public int TagId { get; set; }
            public DateTime? NextDueTimeUtc { get; set; }
            public PreservationStatus Status { get; set; }
            public string Description { get; set; }
            public string ResponsibleCode { get; set; }
            public string ModeTitle { get; set; }
            public string Calloff { get; set; }
            public string AreaCode { get; set; }
            public string DisciplineCode { get; set; }
            public string TagNo { get; set; }
            public string CommPkgNo { get; set; }
            public bool IsVoided { get; set; }
            public string McPkgNo { get; set; }
            public string PurchaseOrderNo { get; set; }
            public string TagFunctionCode { get; set; }
        }

        private class ReqTypeDto
        {
            public int RequirementDefinitionId { get; set; }
            public string RequirementTypeCode { get; set; }
        }
    }
}
