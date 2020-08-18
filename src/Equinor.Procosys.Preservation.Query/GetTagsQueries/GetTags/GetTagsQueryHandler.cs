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
using Equinor.Procosys.Preservation.Domain.Time;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ServiceResult;
using RequirementType = Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate.RequirementType;

namespace Equinor.Procosys.Preservation.Query.GetTagsQueries.GetTags
{
    public class GetTagsQueryHandler : GetTagsQueryBase, IRequestHandler<GetTagsQuery, Result<TagsResult>>
    {
        private readonly IReadOnlyContext _context;
        private readonly int _tagIsNewHours;

        public GetTagsQueryHandler(IReadOnlyContext context, IOptionsMonitor<TagOptions> options)
        {
            _context = context;
            _tagIsNewHours = options.CurrentValue.IsNewHours;
        }

        public async Task<Result<TagsResult>> Handle(GetTagsQuery request, CancellationToken cancellationToken)
        {
            var queryable = CreateQueryableWithFilter(_context, request.ProjectName, request.Filter);

            // count before adding sorting/paging
            var maxAvailable = await queryable.CountAsync(cancellationToken);

            queryable = AddSorting(request.Sorting, queryable);
            queryable = AddPaging(request.Paging, queryable);

            var orderedDtos = await queryable.ToListAsync(cancellationToken);

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
                .ToListAsync(cancellationToken);

            // get Journeys with Steps to be able to calculate ReadyToBeTransferred + NextMode + NextResponsible
            var journeysWithSteps = await (from j in _context.QuerySet<Journey>()
                        .Include(j => j.Steps)
                    where journeyIds.Contains(j.Id)
                    select j)
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
                select m).ToListAsync(cancellationToken);

            var nextResponsibles = await (from r in _context.QuerySet<Responsible>()
                where nextResponsibleIds.Contains(r.Id)
                select r).ToListAsync(cancellationToken);
            
            var reqTypes = await (from rd in _context.QuerySet<RequirementDefinition>()
                    join rt in _context.QuerySet<RequirementType>() on EF.Property<int>(rd, "RequirementTypeId") equals rt.Id
                    where requirementDefinitionIds.Contains(rd.Id)
                    select new ReqTypeDto
                    {
                        RequirementDefinitionId = rd.Id,
                        RequirementTypeCode = rt.Code,
                        RequirementTypeIcon = rt.Icon
                    }
                ).ToListAsync(cancellationToken);

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
                var isReadyToBePreserved = tagWithRequirements.IsReadyToBePreserved();
                var isReadyToBeStarted = tagWithRequirements.IsReadyToBeStarted();
                var isReadyToBeTransferred = tagWithRequirements.IsReadyToBeTransferred(dto.JourneyWithSteps);
                var isReadyToBeCompleted = tagWithRequirements.IsReadyToBeCompleted(dto.JourneyWithSteps);

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
                    isReadyToBePreserved,
                    isReadyToBeStarted,
                    isReadyToBeTransferred,
                    isReadyToBeCompleted,
                    dto.PurchaseOrderNo,
                    requirementDtos,
                    dto.ResponsibleCode,
                    dto.ResponsibleDescription,
                    dto.Status.GetDisplayValue(),
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
