using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.Exceptions;
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
                throw new NotFoundException($"RequirementType with id {request.Id} not found");
            }
            return new SuccessResult<RequirementTypeDto>(new RequirementTypeDto(rt.Id, rt.Code, rt.Title, rt.IsVoided, rt.SortKey));
        }
    }
}
