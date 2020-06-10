using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetTagRequirements
{
    public class GetTagRequirementsQueryHandler : IRequestHandler<GetTagRequirementsQuery, Result<List<RequirementDto>>>
    {
        private readonly IReadOnlyContext _context;

        public GetTagRequirementsQueryHandler(IReadOnlyContext context) => _context = context;

        public async Task<Result<List<RequirementDto>>> Handle(GetTagRequirementsQuery request, CancellationToken cancellationToken)
        {
            // Get tag with all requirements and all previous preservation
            var tagDto = await
                (from tag in _context.QuerySet<Tag>()
                        .Include(t => t.Requirements)
                            .ThenInclude(r => r.PreservationPeriods)
                            .ThenInclude(p => p.PreservationRecord)
                        .Include(t => t.Requirements)
                            .ThenInclude(r => r.PreservationPeriods)
                            .ThenInclude(p => p.FieldValues)
                            .ThenInclude(fv => fv.FieldValueAttachment)
                    join step in _context.QuerySet<Step>() on tag.StepId equals step.Id
                    join mode in _context.QuerySet<Mode>() on step.ModeId equals mode.Id
                 where tag.Id == request.TagId
                 select new
                 {
                     Tag = tag,
                     Mode = mode
                 } ).SingleOrDefaultAsync(cancellationToken);

            if (tagDto == null)
            {
                return new NotFoundResult<List<RequirementDto>>($"{nameof(Tag)} with ID {request.TagId} not found");
            }

            var requirementDefinitionIds = tagDto.Tag.Requirements.Select(r => r.RequirementDefinitionId).ToList();

            // get needed information about requirementType/Definition for all requirement on tag
            var requirementDtos = await
                (from requirementDefinition in _context.QuerySet<RequirementDefinition>().Include(rd => rd.Fields)
                    join requirementType in _context.QuerySet<RequirementType>()
                        on EF.Property<int>(requirementDefinition, "RequirementTypeId") equals requirementType.Id
                    where requirementDefinitionIds.Contains(requirementDefinition.Id)
                    select new
                    {
                        ReqTypeCode = requirementType.Code,
                        ReqTypeTitle = requirementType.Title,
                        RequirementDefinition = requirementDefinition
                    }
                ).ToListAsync(cancellationToken);

            var requirements = tagDto.Tag
                .OrderedRequirements()
                .Select(requirement =>
                {
                    // .Single should be OK here since all requirements for a tag should be to unique Definitions
                    var requirementDto =
                        requirementDtos.Single(rd => rd.RequirementDefinition.Id == requirement.RequirementDefinitionId);

                    var fields = requirementDto
                        .RequirementDefinition
                        .OrderedFields(false)
                        .Select(f =>
                        {
                            var currentValue = requirement.GetCurrentFieldValue(f);
                            var previousValue = requirement.GetPreviousFieldValue(f);
                            return new FieldDto(f, currentValue, previousValue);
                        })
                        .ToList();

                    return new RequirementDto(
                        requirement.Id,
                        requirement.IntervalWeeks,
                        requirement.GetNextDueInWeeks(),
                        requirementDto.ReqTypeCode,
                        requirementDto.ReqTypeTitle,
                        requirementDto.RequirementDefinition.Title,
                        requirement.NextDueTimeUtc,
                        requirement.ReadyToBePreserved,
                        fields,
                        requirement.GetCurrentComment(),
                        requirement.RowVersion.ConvertToString());
                }).ToList();
            
            return new SuccessResult<List<RequirementDto>>(requirements);
        }
    }

}
