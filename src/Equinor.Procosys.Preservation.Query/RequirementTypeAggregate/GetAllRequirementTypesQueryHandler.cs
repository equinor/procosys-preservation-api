using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using MediatR;

namespace Equinor.Procosys.Preservation.Query.RequirementTypeAggregate
{
    public class
        GetAllRequirementTypesQueryHandler : IRequestHandler<GetAllRequirementTypesQuery,
            IEnumerable<RequirementTypeDto>>
    {
        private readonly IRequirementTypeRepository _requirementTypeRepository;

        public GetAllRequirementTypesQueryHandler(IRequirementTypeRepository requirementTypeRepository) =>
            _requirementTypeRepository = requirementTypeRepository;

        public async Task<IEnumerable<RequirementTypeDto>> Handle(GetAllRequirementTypesQuery request,
            CancellationToken cancellationToken)
        {
            var requirementTypes = await _requirementTypeRepository.GetAllAsync();
            return requirementTypes.Select(rt =>
                new RequirementTypeDto(rt.Id,
                    rt.Code,
                    rt.Title,
                    rt.IsVoided,
                    rt.SortKey,
                    rt.RequirementDefinitions.Select(rd =>
                        new RequirementDefinitionDto(rd.Id,
                            rd.Title,
                            rd.IsVoided,
                            rd.DefaultInterval,
                            rd.SortKey,
                            rd.Fields.Select(f => new FieldDto(
                                f.Id,
                                f.Label,
                                f.Unit,
                                f.IsVoided,
                                f.ShowPrevious,
                                f.SortKey))))));
        }
    }
}
