using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Infrastructure;
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
            var tag = await
                (from t in _context.QuerySet<Tag>()
                        .Include(t => t.Requirements)
                        .ThenInclude(r => r.PreservationPeriods)
                        .ThenInclude(p => p.FieldValues)
                    where t.Id == request.Id
                    select t)
                .FirstOrDefaultAsync(cancellationToken);
            
            if (tag == null)
            {
                return new NotFoundResult<List<RequirementDto>>($"Entity with ID {request.Id} not found");
            }

            var requirementDefinitionIds = tag.Requirements.Select(r => r.RequirementDefinitionId).ToList();

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
                    var requirementDto =
                        requirementDtos.Single(rd => rd.RequirementDefinition.Id == requirement.RequirementDefinitionId);

                    var fields = requirementDto
                        .RequirementDefinition
                        .OrderedFields()
                        .Select(f => new FieldDto(f.Id, f.Label, f.FieldType, f.Unit, f.ShowPrevious, null, null)).ToList();

                    return new RequirementDto(
                        requirement.Id,
                        requirementDto.ReqTypeCode,
                        requirementDto.ReqTypeTitle,
                        requirementDto.RequirementDefinition.Title,
                        requirement.NextDueTimeUtc,
                        requirement.ReadyToBePreserved,
                        fields);
                })
                .ToList();
            
            return new SuccessResult<List<RequirementDto>>(requirements);
        }
        
        private class Dto
        {
            public string ReqTypeCode { get; set; }
            public string ReqTypeTitle { get; set; }
            public RequirementDefinition RequirementDefinition { get; set; }
        }
    }

}
