﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetTagRequirements
{
    public class GetTagRequirementsQueryHandler : IRequestHandler<GetTagRequirementsQuery, Result<List<RequirementDetailsDto>>>
    {
        private readonly IReadOnlyContext _context;

        public GetTagRequirementsQueryHandler(IReadOnlyContext context) => _context = context;

        public async Task<Result<List<RequirementDetailsDto>>> Handle(GetTagRequirementsQuery request, CancellationToken cancellationToken)
        {
            // Get tag with all requirements and all previous preservation
            var tag = await
                (from t in _context.QuerySet<Tag>()
                        .Include(t => t.Requirements)
                            .ThenInclude(r => r.PreservationPeriods)
                            .ThenInclude(p => p.PreservationRecord)
                        .Include(t => t.Requirements)
                            .ThenInclude(r => r.PreservationPeriods)
                            .ThenInclude(p => p.FieldValues)
                            .ThenInclude(fv => fv.FieldValueAttachment)
                 where t.Id == request.TagId
                 select t).SingleOrDefaultAsync(cancellationToken);

            if (tag == null)
            {
                return new NotFoundResult<List<RequirementDetailsDto>>($"{nameof(Tag)} with ID {request.TagId} not found");
            }

            var requirementDefinitionIds = tag.Requirements.Select(r => r.RequirementDefinitionId).ToList();

            // get needed information about requirementType/Definition for all requirement on tag
            var requirementDtos = await
                (from requirementDefinition in _context.QuerySet<RequirementDefinition>().Include(rd => rd.Fields)
                    join requirementType in _context.QuerySet<RequirementType>()
                        on EF.Property<int>(requirementDefinition, "RequirementTypeId") equals requirementType.Id
                    where requirementDefinitionIds.Contains(requirementDefinition.Id)
                    select new
                    {
                        RequirementType = requirementType,
                        RequirementDefinition = requirementDefinition
                    }
                ).ToListAsync(cancellationToken);

            var requirements = tag
                .OrderedRequirements(request.IncludeVoided, request.IncludeAllUsages)
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
                            return new FieldDetailsDto(f, currentValue, previousValue);
                        })
                        .ToList();

                    return new RequirementDetailsDto(
                        requirement.Id,
                        requirement.IntervalWeeks,
                        requirement.GetNextDueInWeeks(),
                        new RequirementTypeDetailsDto(
                            requirementDto.RequirementType.Id,
                            requirementDto.RequirementType.Code,
                            requirementDto.RequirementType.Icon,
                            requirementDto.RequirementType.Title), 
                        new RequirementDefinitionDetailDto(
                            requirementDto.RequirementDefinition.Id,
                            requirementDto.RequirementDefinition.Title), 
                        requirement.NextDueTimeUtc,
                        requirement.ReadyToBePreserved,
                        fields,
                        requirement.GetCurrentComment(),
                        requirement.IsVoided,
                        requirement.IsInUse,
                        requirement.RowVersion.ConvertToString());
                }).ToList();

            return new SuccessResult<List<RequirementDetailsDto>>(requirements);
        }
    }

}
