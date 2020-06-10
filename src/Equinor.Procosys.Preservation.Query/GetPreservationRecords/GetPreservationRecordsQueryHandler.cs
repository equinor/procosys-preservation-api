using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using MediatR;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetPreservationRecords
{
    public class GetPreservationRecordsQueryHandler : IRequestHandler<GetPreservationRecordsQuery, Result<List<PreservationRecordDto>>>
    {
        private readonly IReadOnlyContext _context;

        public GetPreservationRecordsQueryHandler(IReadOnlyContext context) => _context = context;

        public async Task<Result<List<PreservationRecordDto>>> Handle(GetPreservationRecordsQuery request, CancellationToken cancellationToken)
        {
            // Get tag with all requirements, all periods, all preservation records
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
                return new NotFoundResult<List<PreservationRecordDto>>($"{nameof(Tag)} with ID {request.TagId} not found");
            }

            var tagReq = await
                (from r in _context.QuerySet<TagRequirement>()
                        .Include(r => r.PreservationPeriods)
                            .ThenInclude(p => p.PreservationRecord)
                    where r.Id == request.RequirementId
                    select r).SingleOrDefaultAsync(cancellationToken);

            if (tagReq == null)
            {
                return new NotFoundResult<List<PreservationRecordDto>>($"{nameof(TagRequirement)} with ID {request.RequirementId} not found");
            }

            var requirementDefinitionIds = tag.Requirements.Select(r => r.RequirementDefinitionId).ToList();
            var tagRequirements = tag.Requirements.Select(r => r.Id).ToList();
            var preservationPeriodIds = tagReq.PreservationPeriods
                .Select(r => r.Id)
                .ToList();


            // get needed information about requirementType/Definition for all requirement on tag
            var requirementDtos = await
                (from requirementDefinition in _context.QuerySet<RequirementDefinition>().Include(rd => rd.Fields)
                    join requirementType in _context.QuerySet<RequirementType>()
                        on EF.Property<int>(requirementDefinition, "RequirementTypeId") equals requirementType.Id
                    where requirementDefinitionIds.Contains(requirementDefinition.Id)
                    select new Dto
                    {
                        ReqTypeCode = requirementType.Code,
                        ReqTypeTitle = requirementType.Title,
                        RequirementDefinition = requirementDefinition
                    }
                ).ToListAsync(cancellationToken);

            var requirements = tag
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

                    return new PreservationRecordDto(
                        requirement.Id,
                        requirement.RowVersion.ConvertToString());
                }).ToList();
            
            return new SuccessResult<List<PreservationRecordDto>>(requirements);
        }
        
        private class Dto
        {
            public string ReqTypeCode { get; set; }
            public string ReqTypeTitle { get; set; }
            public RequirementDefinition RequirementDefinition { get; set; }
        }
    }

}
