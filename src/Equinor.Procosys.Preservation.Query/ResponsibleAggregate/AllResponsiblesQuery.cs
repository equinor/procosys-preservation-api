using System.Collections.Generic;
using MediatR;

namespace Equinor.Procosys.Preservation.Query.ResponsibleAggregate
{
    public class AllResponsiblesQuery : IRequest<List<ResponsibleDto>>
    {
    }
}
