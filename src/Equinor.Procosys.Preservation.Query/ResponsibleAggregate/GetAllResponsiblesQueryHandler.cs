using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.ResponsibleAggregate
{
    public class GetAllResponsiblesQueryHandler : IRequestHandler<GetAllResponsiblesQuery, Result<IEnumerable<ResponsibleDto>>>
    {
        private readonly IResponsibleRepository _responsibleRepository;

        public GetAllResponsiblesQueryHandler(IResponsibleRepository responsibleRepository) => _responsibleRepository = responsibleRepository;

        public async Task<Result<IEnumerable<ResponsibleDto>>> Handle(GetAllResponsiblesQuery request, CancellationToken cancellationToken)
        {
            var responsibles = await _responsibleRepository.GetAllAsync();
            return new SuccessResult<IEnumerable<ResponsibleDto>>(responsibles.Select(responsible => new ResponsibleDto(responsible.Id, responsible.Code)));
        }
    }
}
