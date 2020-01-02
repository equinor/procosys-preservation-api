using System.Collections.Generic;
using MediatR;

namespace Equinor.Procosys.Preservation.Query.ModeAggregate
{
    public class GetAllModesQuery : IRequest<IEnumerable<ModeDto>>
    {
        public GetAllModesQuery()
        {
        }
    }
}
