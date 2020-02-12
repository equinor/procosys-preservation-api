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

            var requirementDefinitions = await
                (from rd in _context.QuerySet<RequirementDefinition>()
                        .Include(rd => rd.Fields)
                    where requirementDefinitionIds.Contains(rd.Id)
                    select rd
                ).ToListAsync(cancellationToken);

            var requirements = tag.Requirements.Select(requirement =>
            {
                var requirementDefinition =
                    requirementDefinitions.Single(rd => rd.Id == requirement.RequirementDefinitionId);

                var fields = requirementDefinition
                    .Fields
                    .Where(f => !f.IsVoided)
                    .OrderBy(f => f.SortKey)
                    .Select(f => new FieldDto(f.Id, f.Label, f.FieldType, f.Unit, f.ShowPrevious)).ToList();

                return new RequirementDto(
                    requirement.Id,
                    requirement.NextDueTimeUtc,
                    requirement.ReadyToBePreserved,
                    fields);
            }).ToList();
            
            return new SuccessResult<List<RequirementDto>>(requirements);
        }
    }
}
