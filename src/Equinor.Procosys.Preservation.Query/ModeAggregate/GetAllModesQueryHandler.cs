using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using MediatR;

namespace Equinor.Procosys.Preservation.Query.ModeAggregate
{
    public class GetAllModesQueryHandler : IRequestHandler<GetAllModesQuery, IEnumerable<ModeDto>>
    {
        private readonly IModeRepository _modeRepository;

        public GetAllModesQueryHandler(IModeRepository modeRepository)
        {
            _modeRepository = modeRepository;
        }

        public async Task<IEnumerable<ModeDto>> Handle(GetAllModesQuery request, CancellationToken cancellationToken)
        {
            var modes = await _modeRepository.GetAllAsync();
            return modes.Select(mode => new ModeDto(mode.Id, mode.Title));
        }
    }
}
