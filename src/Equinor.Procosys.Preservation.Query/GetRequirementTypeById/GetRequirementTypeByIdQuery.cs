using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetRequirementTypeById
{
    public class GetRequirementTypeByIdQuery : IRequest<Result<RequirementTypeDetailsDto>>
    {
        public GetRequirementTypeByIdQuery(int id) => Id = id;

        public int Id { get; }
    }
}
