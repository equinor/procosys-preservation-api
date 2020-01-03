using MediatR;

namespace Equinor.Procosys.Preservation.Query.RequirementTypeAggregate
{
    public class GetRequirementTypeByIdQuery : IRequest<RequirementTypeDto>
    {
        public GetRequirementTypeByIdQuery(int id) => Id = id;

        public int Id { get; }
    }
}
