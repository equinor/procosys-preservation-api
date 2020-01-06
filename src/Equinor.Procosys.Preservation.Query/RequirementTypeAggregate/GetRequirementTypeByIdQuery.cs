using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.RequirementTypeAggregate
{
    public class GetRequirementTypeByIdQuery : IRequest<Result<RequirementTypeDto>>
    {
        public GetRequirementTypeByIdQuery(int id) => Id = id;

        public int Id { get; }
    }
}
