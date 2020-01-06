using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.Exceptions;
using MediatR;

namespace Equinor.Procosys.Preservation.Query.RequirementTypeAggregate
{
    public class GetRequirementTypeByIdQueryHandler : IRequestHandler<GetRequirementTypeByIdQuery, RequirementTypeDto>
    {
        private readonly IRequirementTypeRepository _requirementTypeRepository;

        public GetRequirementTypeByIdQueryHandler(IRequirementTypeRepository requirementTypeRepository) => _requirementTypeRepository = requirementTypeRepository;

        public async Task<RequirementTypeDto> Handle(GetRequirementTypeByIdQuery request, CancellationToken cancellationToken)
        {
            var rt = await _requirementTypeRepository.GetByIdAsync(request.Id);
            if (rt == null)
            {
                throw new NotFoundException($"RequirementType with id {request.Id} not found");
            }

            return new RequirementTypeDto(rt.Id,
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
                            f.SortKey)))));
        }
    }
}
