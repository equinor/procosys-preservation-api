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

        public GetAllRequirementTypesQueryHandler(IRequirementTypeRepository requirementTypeRepository) =>
            _requirementTypeRepository = requirementTypeRepository;

        public async Task<Result<IEnumerable<RequirementTypeDto>>> Handle(GetAllRequirementTypesQuery request, CancellationToken cancellationToken)
        {
            var requirementTypes = await _requirementTypeRepository.GetAllAsync();

            var dtos = 
                requirementTypes.Where(rt => rt.IsVoided == false || request.IncludeVoided).Select(rt =>
                new RequirementTypeDto(rt.Id,
                    rt.Code,
                    rt.Title,
                    rt.IsVoided,
                    rt.SortKey,
                    rt.RequirementDefinitions.Where(rd => rd.IsVoided == false || request.IncludeVoided).Select(rd =>
                        new RequirementDefinitionDto(rd.Id,
                            rd.Title,
                            rd.IsVoided,
                            rd.DefaultInterval,
                            rd.SortKey,
                            rd.Fields.Where(f => f.IsVoided == false || request.IncludeVoided).Select(f => new FieldDto(
                                f.Id,
                                f.Label,
                                f.Unit,
                                f.IsVoided,
                                f.ShowPrevious,
                                f.SortKey))))));

            return new SuccessResult<IEnumerable<RequirementTypeDto>>(dtos);
        }
    }
}
