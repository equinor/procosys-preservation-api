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
            var responsibles = new List<ResponsibleDto>
            {
                new ResponsibleDto(1, "ASHS"),
                new ResponsibleDto(2, "KSI"),
                new ResponsibleDto(3, "EQLC"),
                new ResponsibleDto(4, "ACPI"),
            };

            return Task.FromResult(responsibles);
        }
    }
}
