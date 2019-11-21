using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Equinor.Procosys.Preservation.Query.ResponsibleAggregate
{
    public class AllResponsiblesQueryHandler : IRequestHandler<AllResponsiblesQuery, List<ResponsibleDto>>
    {
        public Task<List<ResponsibleDto>> Handle(AllResponsiblesQuery request, CancellationToken cancellationToken)
        {
            List<ResponsibleDto> responsibles = new List<ResponsibleDto>
            {
                new ResponsibleDto { Name = "ASHS" },
                new ResponsibleDto { Name = "KSI" },
                new ResponsibleDto { Name = "EQLC" },
                new ResponsibleDto { Name = "ACPI" },
            };

            return Task.FromResult(responsibles);
        }
    }
}
