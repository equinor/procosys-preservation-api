using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.ResponsibleAggregate
{
    public class AllResponsiblesQueryHandler : IRequestHandler<AllResponsiblesQuery, Result<List<ResponsibleDto>>>
    {
        public Task<Result<List<ResponsibleDto>>> Handle(AllResponsiblesQuery request, CancellationToken cancellationToken)
        {
            var responsibles = new List<ResponsibleDto>
            {
                new ResponsibleDto(1, "ASHS"),
                new ResponsibleDto(2, "KSI"),
                new ResponsibleDto(3, "EQLC"),
                new ResponsibleDto(4, "ACPI"),
            };

            var ras = new SuccessResult<List<ResponsibleDto>>(responsibles);
            return Task.FromResult(ras as Result<List<ResponsibleDto>>);
        }
    }
}
