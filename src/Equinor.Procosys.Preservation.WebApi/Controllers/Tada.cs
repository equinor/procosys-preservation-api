using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.WebApi.Controllers
{
    public class Tada : IRequest<Unit>
    {
    }

    public class TadaHandler : IRequestHandler<Tada>
    {
        public Task<Unit> Handle(Tada request, CancellationToken cancellationToken)
        {
            return Task.FromResult(Unit.Value);
        }
    }
}
