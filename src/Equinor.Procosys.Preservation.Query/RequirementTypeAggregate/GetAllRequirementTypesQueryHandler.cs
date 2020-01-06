using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.RequirementTypeAggregate
{
    public class GetAllRequirementTypesQueryHandler : IRequestHandler<GetAllRequirementTypesQuery, Result<IEnumerable<RequirementTypeDto>>>
    {
        private readonly IRequirementTypeRepository _requirementTypeRepository;

        public GetAllRequirementTypesQueryHandler(IRequirementTypeRepository requirementTypeRepository) => _requirementTypeRepository = requirementTypeRepository;

        public async Task<Result<IEnumerable<RequirementTypeDto>>> Handle(GetAllRequirementTypesQuery request, CancellationToken cancellationToken)
        {
            var requirementTypes = await _requirementTypeRepository.GetAllAsync();
            return new SuccessResult<IEnumerable<RequirementTypeDto>>(requirementTypes.Select(rt => new RequirementTypeDto(rt.Id, rt.Code, rt.Title, rt.IsVoided, rt.SortKey)));
        }
    }
}
