using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.ModeAggregate
{
    public class GetAllModesQueryHandler : IRequestHandler<GetAllModesQuery, Result<IEnumerable<ModeDto>>>
    {
        private readonly IModeRepository _modeRepository;

        public GetAllModesQueryHandler(IModeRepository modeRepository) => _modeRepository = modeRepository;

        public async Task<Result<IEnumerable<ModeDto>>> Handle(GetAllModesQuery request, CancellationToken cancellationToken)
        {
            var modes = await _modeRepository.GetAllAsync();
            return new SuccessResult<IEnumerable<ModeDto>>(modes.Select(mode => new ModeDto(mode.Id, mode.Title)));
        }
    }
}
