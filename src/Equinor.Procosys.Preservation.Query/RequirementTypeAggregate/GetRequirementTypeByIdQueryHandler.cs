using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.RequirementTypeAggregate
{
    public class GetRequirementTypeByIdQueryHandler : IRequestHandler<GetRequirementTypeByIdQuery, Result<RequirementTypeDto>>
    {
        private readonly IRequirementTypeRepository _requirementTypeRepository;

        public GetRequirementTypeByIdQueryHandler(IRequirementTypeRepository requirementTypeRepository) => _requirementTypeRepository = requirementTypeRepository;

        public async Task<Result<RequirementTypeDto>> Handle(GetRequirementTypeByIdQuery request, CancellationToken cancellationToken)
        {
            var rt = await _requirementTypeRepository.GetByIdAsync(request.Id);
            if (rt == null)
            {
                return new NotFoundResult<RequirementTypeDto>(Strings.EntityNotFound(nameof(RequirementType), request.Id));
            }

            var dto = new RequirementTypeDto(rt.Id,
                rt.Code,
                rt.Title,
                rt.IsVoided,
                rt.SortKey,
                rt.RequirementDefinitions.Select(rd =>
                    new RequirementDefinitionDto(rd.Id,
                        rd.Title,
                        rd.IsVoided,
                        rd.DefaultIntervalWeeks,
                        rd.SortKey,
                        rd.Fields.Select(f => new FieldDto(
                            f.Id,
                            f.Label,
                            f.IsVoided,
                            f.FieldType,
                            f.SortKey,
                            f.Unit,
                            f.ShowPrevious)))));

            return new SuccessResult<RequirementTypeDto>(dto);
        }
    }
}
