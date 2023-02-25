using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.ProCoSys.Auth.Time;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ServiceResult;
using RequirementType = Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate.RequirementType;

namespace Equinor.ProCoSys.Preservation.Query.GetTagsQueries.GetTags
{
    public class GetTagsQueryHandler : GetTagsQueryBase, IRequestHandler<GetTagsQuery, Result<TagsResult>>
    {
        private readonly IReadOnlyContext _context;
        private readonly int _tagIsNewHours;
        private readonly DateTime _utcNow;

        public GetTagsQueryHandler(IReadOnlyContext context, IOptionsSnapshot<TagOptions> options)
        {
            _context = context;
            _tagIsNewHours = options.Value.IsNewHours;
            _utcNow = TimeService.UtcNow;
        }

        public async Task<Result<TagsResult>> Handle(GetTagsQuery request, CancellationToken cancellationToken)
        {
            var queryable = CreateQueryableWithFilter(_context, request.ProjectName, request.Filter, _utcNow);

            // count before adding sorting/paging
            var maxAvailable = await queryable
                .TagWith("GetTagsQueryHandler: maxAvailable")
                .CountAsync(cancellationToken);
            
            queryable = AddSorting(request.Sorting, queryable);
            queryable = AddPaging(request.Paging, queryable);

            var orderedDtos = await queryable
                .TagWith("GetTagsQueryHandler: orderedDtos")
                .ToListAsync(cancellationToken);

            if (!orderedDtos.Any())
            {
                return new SuccessResult<TagsResult>(new TagsResult(maxAvailable, null));
            }

            var tagsIds = orderedDtos.Select(dto => dto.TagId);
            var journeyIds = orderedDtos.Select(dto => dto.JourneyId).Distinct();

            // get tags again, including Requirements and PreservationPeriods. See comment in CreateQueryableWithFilter regarding Include and EF
            var tagsWithRequirements = await (from tag in _context.QuerySet<Tag>()
                        .Include(t => t.Requirements)
                        .ThenInclude(r => r.PreservationPeriods)
                    where tagsIds.Contains(tag.Id)
                    select tag)
                .TagWith("GetTagsQueryHandler: tagsWithRequirements")
                .ToListAsync(cancellationToken);

            // get Journeys with Steps to be able to calculate ReadyToBeTransferred + NextMode + NextResponsible
            var journeysWithSteps = await (from j in _context.QuerySet<Journey>()
                        .Include(j => j.Steps)
                    where journeyIds.Contains(j.Id)
                    select j)
                .TagWith("GetTagsQueryHandler: journeysWithSteps")
                .ToListAsync(cancellationToken);

            // enrich DTO to be able to get distinct NextSteps to query database for distinct NextMode + NextResponsible
            foreach (var dto in orderedDtos)
            {
                dto.JourneyWithSteps = journeysWithSteps.Single(j => j.Id == dto.JourneyId);
                dto.NextStep = dto.JourneyWithSteps.GetNextStep(dto.StepId);
            }

            var nextModeIds = orderedDtos.Where(dto => dto.NextStep != null).Select(dto => dto.NextStep.ModeId).Distinct();
            var nextResponsibleIds = orderedDtos.Where(dto => dto.NextStep != null).Select(dto => dto.NextStep.ResponsibleId).Distinct();
            var requirementDefinitionIds = tagsWithRequirements.SelectMany(t => t.Requirements).Select(r => r.RequirementDefinitionId).Distinct();

            var nextModes = await (from m in _context.QuerySet<Mode>()
                    where nextModeIds.Contains(m.Id)
                select m)
                .TagWith("GetTagsQueryHandler: nextModes")
                .ToListAsync(cancellationToken);

            var nextResponsibles = await (from r in _context.QuerySet<Responsible>()
                where nextResponsibleIds.Contains(r.Id)
                select r)
                .TagWith("GetTagsQueryHandler: nextResponsibles")
                .ToListAsync(cancellationToken);
            
            var reqTypes = await (from rd in _context.QuerySet<RequirementDefinition>()
                                  join rt in _context.QuerySet<RequirementType>() on EF.Property<int>(rd, "RequirementTypeId") equals rt.Id
                    where requirementDefinitionIds.Contains(rd.Id)
                    select new ReqTypeDto
                    {
                        RequirementDefinitionId = rd.Id,
                        RequirementTypeCode = rt.Code,
                        RequirementTypeIcon = rt.Icon
                    }
                )
                .TagWith("GetTagsQueryHandler: reqTypes")
                .ToListAsync(cancellationToken);

            var result = CreateResult(
                maxAvailable,
                orderedDtos,
                tagsWithRequirements,
                reqTypes, 
                nextModes,
                nextResponsibles);

            return new SuccessResult<TagsResult>(result);
        }

        private TagsResult CreateResult(
            int maxAvailable,
            List<TagForQueryDto> orderedDtos,
            List<Tag> tagsWithRequirements,
            List<ReqTypeDto> reqTypes,
            List<Mode> nextModes,
            List<Responsible> nextResponsibles)
        {
            var tags = orderedDtos.Select(dto =>
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
                                reqTypeDto.RequirementTypeIcon,
                                r.NextDueTimeUtc,
                                r.GetNextDueInWeeks(),
                                r.IsReadyAndDueToBePreserved());
                        })
                    .ToList();

                var isNew = IsNew(tagWithRequirements);
                var isReadyToBeEdited = tagWithRequirements.IsReadyToBeEdited();
                var isReadyToBePreserved = tagWithRequirements.IsReadyToBePreserved();
                var isReadyToBeStarted = tagWithRequirements.IsReadyToBeStarted();
                var isReadyToBeTransferred = tagWithRequirements.IsReadyToBeTransferred(dto.JourneyWithSteps);
                var isReadyToBeCompleted = tagWithRequirements.IsReadyToBeCompleted(dto.JourneyWithSteps);
                var isReadyToBeRescheduled = tagWithRequirements.IsReadyToBeRescheduled();
                var isReadyToBeDuplicated = tagWithRequirements.IsReadyToBeDuplicated();
                var isReadyToUndoStarted = tagWithRequirements.IsReadyToUndoStarted();
                var isReadyToBeSetInService = tagWithRequirements.IsReadyToBeSetInService();

                var nextMode = tagWithRequirements.FollowsAJourney && dto.NextStep != null 
                    ? nextModes.Single(m => m.Id == dto.NextStep.ModeId)
                    : null;
                
                var nextResponsible = tagWithRequirements.FollowsAJourney && dto.NextStep != null
                    ? nextResponsibles.Single(m => m.Id == dto.NextStep.ResponsibleId)
                    : null;

                return new TagDto(dto.TagId,
                    dto.GetActionStatus(),
                    dto.AreaCode,
                    dto.CalloffNo,
                    dto.CommPkgNo,
                    dto.DisciplineCode,
                    isNew,
                    dto.IsVoided,
                    dto.McPkgNo,
                    dto.ModeTitle,
                    nextMode?.Title,
                    nextResponsible?.Code,
                    isReadyToBeEdited,
                    isReadyToBePreserved,
                    isReadyToBeStarted,
                    isReadyToBeTransferred,
                    isReadyToBeCompleted,
                    isReadyToBeRescheduled,
                    isReadyToBeDuplicated,
                    isReadyToUndoStarted,
                    isReadyToBeSetInService,
                    dto.PurchaseOrderNo,
                    requirementDtos,
                    dto.ResponsibleCode,
                    dto.ResponsibleDescription,
                    dto.Status,
                    dto.StorageArea,
                    dto.TagFunctionCode,
                    dto.Description,
                    dto.TagNo,
                    dto.TagType,
                    dto.RowVersion.ConvertToString());
            });
            var result = new TagsResult(maxAvailable, tags);
            return result;
        }

        private bool IsNew(Tag tag)
        {
            var lastTimeIsNew = tag.CreatedAtUtc.AddHours(_tagIsNewHours);
            return TimeService.UtcNow < lastTimeIsNew;
        }

        private IQueryable<TagForQueryDto> AddPaging(Paging paging, IQueryable<TagForQueryDto> queryable)
        {
            queryable = queryable
                .Skip(paging.Page * paging.Size)
                .Take(paging.Size);
            return queryable;
        }

        private class ReqTypeDto
        {
            public int RequirementDefinitionId { get; set; }
            public string RequirementTypeCode { get; set; }
            public RequirementTypeIcon RequirementTypeIcon { get; set; }
        }
    }
}
