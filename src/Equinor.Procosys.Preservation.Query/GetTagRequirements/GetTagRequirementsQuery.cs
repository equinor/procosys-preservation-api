using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetTagRequirements
{
    public class GetTagRequirementsQuery : IRequest<Result<List<RequirementDto>>>
    {
        public GetTagRequirementsQuery(int id) => Id = id;

        public int Id { get; }
    }
}
