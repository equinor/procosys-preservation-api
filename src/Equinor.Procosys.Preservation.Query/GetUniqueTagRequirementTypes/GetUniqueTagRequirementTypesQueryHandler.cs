using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetUniqueTagRequirementTypes
{
    public class GetUniqueTagRequirementTypesQueryHandler : IRequestHandler<GetUniqueTagRequirementTypesQuery, Result<List<RequirementTypeDto>>>
    {
        private readonly IReadOnlyContext _context;

        public GetUniqueTagRequirementTypesQueryHandler(IReadOnlyContext context) => _context = context;

        public async Task<Result<List<RequirementTypeDto>>> Handle(GetUniqueTagRequirementTypesQuery request, CancellationToken cancellationToken)
        {
            var requirementTypes = await
                (from requirementType in _context.QuerySet<RequirementType>()
                        join requirementDefinition in _context.QuerySet<RequirementDefinition>()
                            on requirementType.Id equals EF.Property<int>(requirementDefinition, "RequirementTypeId")
                        join requirement in _context.QuerySet<Requirement>()
                            on requirementDefinition.Id equals requirement.RequirementDefinitionId
                        join tag in _context.QuerySet<Tag>()
                            on EF.Property<int>(requirement, "TagId") equals tag.Id
                        join project in _context.QuerySet<Project>()
                            on EF.Property<int>(tag, "ProjectId") equals project.Id
                 where project.Name == request.ProjectName
                 select new RequirementTypeDto(
                     requirementType.Id,
                     requirementType.Code,
                     requirementType.Title))
                .Distinct()
                .ToListAsync(cancellationToken);
            
            return new SuccessResult<List<RequirementTypeDto>>(requirementTypes);
        }
    }
}
