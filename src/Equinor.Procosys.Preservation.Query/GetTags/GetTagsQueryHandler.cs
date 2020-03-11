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

        public GetTagsQueryHandler(IReadOnlyContext context) => _context = context;

        public async Task<Result<TagsResult>> Handle(GetTagsQuery request, CancellationToken cancellationToken)
        {
            var queryable = CreateQueryableWithFilter(request);

            // count before adding sorting/paging
            var countTask = queryable.CountAsync(cancellationToken);
            var maxAvailable = await countTask;

            queryable = AddSorting(request.Sorting, queryable);
            queryable = AddPaging(request.Paging, queryable);

            var resultTask = queryable.ToListAsync(cancellationToken);

            var orderedDtos = await resultTask;

            if (!orderedDtos.Any())
            {
                return new SuccessResult<TagsResult>(
                    new TagsResult
                    {
                        MaxAvailable = maxAvailable,
                        Tags = new List<TagDto>()
                    });
            }

            var tagsIds = orderedDtos.Select(dto => dto.TagId);
            var journeyIds = orderedDtos.Select(dto => dto.JourneyId).Distinct();

            // get tags again, including Requirements and PreservationPeriods. See comment in CreateQueryable regarding Include and EF
            var tagsWithRequirementsTask = (from tag in _context.QuerySet<Tag>().Include(t => t.Requirements).ThenInclude(r => r.PreservationPeriods)
                    where tagsIds.Contains(tag.Id)
                    select tag)
                .ToListAsync(cancellationToken);
            var tagsWithRequirements = await tagsWithRequirementsTask;

            // get Journeys with Steps to be able to calculate ReadyToBeTransferred + NextMode + NextResponsible
            var journeysWithStepsTask = (from j in _context.QuerySet<Journey>().Include(j => j.Steps)
                    where journeyIds.Contains(j.Id)
                    select j)
                .ToListAsync(cancellationToken);
            var journeysWithSteps = await journeysWithStepsTask;

            // enrich DTO to be able to get distinct NextSteps to query database for distinct NextMode + NextResponsible
            foreach (var dto in orderedDtos)
            {
                dto.JourneyWithSteps = journeysWithSteps.Single(j => j.Id == dto.JourneyId);
                dto.NextStep = dto.JourneyWithSteps.GetNextStep(dto.StepId);
            }

            var nextModeIds = orderedDtos.Where(dto => dto.NextStep != null).Select(dto => dto.NextStep.ModeId).Distinct();
            var nextResponsibleIds = orderedDtos.Where(dto => dto.NextStep != null).Select(dto => dto.NextStep.ResponsibleId).Distinct();
            var requirementDefinitionIds = tagsWithRequirements.SelectMany(t => t.Requirements).Select(r => r.RequirementDefinitionId).Distinct();

            var nextModesTask = (from m in _context.QuerySet<Mode>()
                where nextModeIds.Contains(m.Id)
                select m).ToListAsync(cancellationToken);
            var nextModes = await nextModesTask;

            var nextResponsibleTask = (from r in _context.QuerySet<Responsible>()
                where nextResponsibleIds.Contains(r.Id)
                select r).ToListAsync(cancellationToken);
            var nextResponsibles = await nextResponsibleTask;
            
            var reqTypesTask = (from rd in _context.QuerySet<RequirementDefinition>()
                    join rt in _context.QuerySet<RequirementType>() on EF.Property<int>(rd, "RequirementTypeId") equals rt.Id
                    where requirementDefinitionIds.Contains(rd.Id)
                    select new ReqTypeDto
                    {
                        RequirementDefinitionId = rd.Id,
                        RequirementTypeCode = rt.Code
                    }
                ).ToListAsync(cancellationToken);
            var reqTypes = await reqTypesTask;

            var result = CreateResult(maxAvailable, orderedDtos, tagsWithRequirements, reqTypes, nextModes, nextResponsibles);

            return new SuccessResult<TagsResult>(result);
        }

        private static TagsResult CreateResult(
            int maxAvailable,
            List<Dto> orderedDtos,
            List<Tag> tagsWithRequirements,
            List<ReqTypeDto> reqTypes,
            List<Mode> nextModes,
            List<Responsible> nextResponsibles)
        {
            var result = new TagsResult
            {
                MaxAvailable = maxAvailable,
                Tags = orderedDtos.Select(dto =>
                {
                    var tagWithRequirements = tagsWithRequirements.Single(t => t.Id == dto.TagId);
                    var requirementDtos = tagWithRequirements.OrderedRequirements().Select(
                            r =>
                            {
                                var reqTypeDto =
                                    reqTypes.Single(innerDto =>
                                        innerDto.RequirementDefinitionId == r.RequirementDefinitionId);
                                return new RequirementDto(
                                    r.Id,
                                    reqTypeDto.RequirementTypeCode,
                                    r.NextDueTimeUtc,
                                    r.GetNextDueInWeeks(),
                                    r.IsReadyAndDueToBePreserved());
                            })
                        .ToList();
                    var isReadyToBePreserved = tagWithRequirements.IsReadyToBePreserved();
                    var isReadyToBeStarted = tagWithRequirements.IsReadyToBeStarted();
                    var isReadyToBeTransferred = tagWithRequirements.IsReadyToBeTransferred(dto.JourneyWithSteps);

                    var nextMode = dto.NextStep != null ? nextModes.Single(m => m.Id == dto.NextStep.ModeId) : null;
                    var nextResponsible = dto.NextStep != null
                        ? nextResponsibles.Single(m => m.Id == dto.NextStep.ResponsibleId)
                        : null;

                    return new TagDto(dto.TagId,
                        dto.AreaCode,
                        dto.Calloff,
                        dto.CommPkgNo,
                        dto.DisciplineCode,
                        false,
                        dto.IsVoided,
                        dto.McPkgNo,
                        dto.ModeTitle,
                        nextMode?.Title,
                        nextResponsible?.Code,
                        isReadyToBePreserved,
                        isReadyToBeStarted,
                        isReadyToBeTransferred,
                        dto.PurchaseOrderNo,
                        requirementDtos,
                        dto.ResponsibleCode,
                        dto.Status,
                        dto.TagFunctionCode,
                        dto.Description,
                        dto.TagNo,
                        dto.TagType);
                })
            };
            return result;
        }

        private IQueryable<Dto> CreateQueryableWithFilter(GetTagsQuery request)
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
                    join reqType in _context.QuerySet<RequirementType>() on EF.Property<int>(reqDef, "RequirementTypeId") equals
                        reqType.Id
                    where EF.Property<int>(req, "TagId") == tag.Id &&
                          request.Filter.RequirementTypeIds.Contains(reqType.Id)
                    select reqType.Id).Any()
                where project.Name == request.Filter.ProjectName &&
                      (!request.Filter.PreservationStatus.HasValue || tag.Status == request.Filter.PreservationStatus.Value) &&
                      (string.IsNullOrEmpty(request.Filter.TagNoStartsWith) ||
                       tag.TagNo.StartsWith(request.Filter.TagNoStartsWith)) &&
                      (string.IsNullOrEmpty(request.Filter.CommPkgNoStartsWith) ||
                       tag.CommPkgNo.StartsWith(request.Filter.CommPkgNoStartsWith)) &&
                      (string.IsNullOrEmpty(request.Filter.McPkgNoStartsWith) ||
                       tag.McPkgNo.StartsWith(request.Filter.McPkgNoStartsWith)) &&
                      (string.IsNullOrEmpty(request.Filter.PurchaseOrderNoStartsWith) ||
                       tag.PurchaseOrderNo.StartsWith(request.Filter.PurchaseOrderNoStartsWith)) &&
                      (string.IsNullOrEmpty(request.Filter.CallOffStartsWith) ||
                       tag.Calloff.StartsWith(request.Filter.CallOffStartsWith)) &&
                      (!request.Filter.RequirementTypeIds.Any() || reqTypeFiltered) &&
                      (!request.Filter.TagFunctionCodes.Any() ||
                       request.Filter.TagFunctionCodes.Contains(tag.TagFunctionCode)) &&
                      (!request.Filter.DisciplineCodes.Any() || request.Filter.DisciplineCodes.Contains(tag.DisciplineCode)) &&
                      (!request.Filter.ResponsibleIds.Any() || request.Filter.ResponsibleIds.Contains(responsible.Id)) &&
                      (!request.Filter.JourneyIds.Any() || request.Filter.JourneyIds.Contains(journey.Id)) &&
                      (!request.Filter.ModeIds.Any() || request.Filter.ModeIds.Contains(mode.Id)) &&
                      (!request.Filter.StepIds.Any() || request.Filter.StepIds.Contains(step.Id))
                select new Dto
                {
                    AreaCode = tag.AreaCode,
                    Calloff = tag.Calloff,
                    CommPkgNo = tag.CommPkgNo,
                    Description = tag.Description,
                    DisciplineCode = tag.DisciplineCode,
                    IsVoided = tag.IsVoided,
                    JourneyId = journey.Id,
                    ModeTitle = mode.Title,
                    McPkgNo = tag.McPkgNo,
                    NextDueTimeUtc = tag.NextDueTimeUtc,
                    PurchaseOrderNo = tag.PurchaseOrderNo,
                    ResponsibleCode = responsible.Code,
                    Status = tag.Status,
                    StepId = step.Id,
                    TagFunctionCode = tag.TagFunctionCode,
                    TagId = tag.Id,
                    TagNo = tag.TagNo,
                    TagType = tag.TagType
                };
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
            switch (sorting.Direction)
            {
                case SortingDirection.Asc:
                    switch (sorting.Column)
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
                    switch (sorting.Column)
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
            public string AreaCode { get; set; }
            public string Calloff { get; set; }
            public string CommPkgNo { get; set; }
            public string Description { get; set; }
            public string DisciplineCode { get; set; }
            public bool IsVoided { get; set; }
            public int JourneyId { get; set; }
            public string McPkgNo { get; set; }
            public string ModeTitle { get; set; }
            public DateTime? NextDueTimeUtc { get; set; }
            public string PurchaseOrderNo { get; set; }
            public string ResponsibleCode { get; set; }
            public PreservationStatus Status { get; set; }
            public int StepId { get; set; }
            public int TagId { get; set; }
            public string TagFunctionCode { get; set; }
            public string TagNo { get; set; }
            public TagType TagType { get; set; }
            
            public Journey JourneyWithSteps { get; set; }
            public Step NextStep { get; set; }
        }

        private class ReqTypeDto
        {
            public int RequirementDefinitionId { get; set; }
            public string RequirementTypeCode { get; set; }
        }
    }
}
