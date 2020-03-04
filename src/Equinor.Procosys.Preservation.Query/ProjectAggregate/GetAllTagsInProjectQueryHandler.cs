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

namespace Equinor.Procosys.Preservation.Query.ProjectAggregate
{
    public class GetAllTagsInProjectQueryHandler : IRequestHandler<GetAllTagsInProjectQuery, Result<IEnumerable<TagDto>>>
    {
        private readonly IReadOnlyContext _context;
        private readonly ITimeService _timeService;

        public GetAllTagsInProjectQueryHandler(
            IReadOnlyContext context,
            ITimeService timeService)
        {
            _timeService = timeService;
            _context = context;
        }

        public async Task<Result<IEnumerable<TagDto>>> Handle(GetAllTagsInProjectQuery request, CancellationToken cancellationToken)
        {
            var unOrderedDtos = await (from tag in _context.QuerySet<Tag>().Include(t => t.Requirements).ThenInclude(r => r.PreservationPeriods)
                join step in _context.QuerySet<Step>() on tag.StepId equals step.Id
                join journey in _context.QuerySet<Journey>().Include(j => j.Steps) on EF.Property<int>(step, "JourneyId") equals journey.Id
                join mode in _context.QuerySet<Mode>() on step.ModeId equals mode.Id
                join responsible in _context.QuerySet<Responsible>() on step.ResponsibleId equals responsible.Id
                join project in _context.QuerySet<Project>() on EF.Property<int>(tag, "ProjectId") equals project.Id
                where project.Name == request.ProjectName
                select new Dto
                {
                    Tag = tag,
                    Responsible = responsible,
                    Mode = mode,
                    Journey = journey
                }).ToListAsync(cancellationToken);

            if (!unOrderedDtos.Any())
            {
                return new SuccessResult<IEnumerable<TagDto>>(new List<TagDto>());
            }

            var orderedDtos = unOrderedDtos
                .OrderByDescending(dto => dto.Tag.NextDueTimeUtc.HasValue)
                .ThenBy(dto => dto.Tag.NextDueTimeUtc)
                .ThenBy(dto => dto.Tag.Status)
                .ThenBy(dto => dto.Tag.Id)
                .ToList();

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
                    false,
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
