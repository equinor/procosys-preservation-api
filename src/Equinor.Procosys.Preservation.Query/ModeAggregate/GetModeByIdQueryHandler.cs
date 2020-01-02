using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using MediatR;

namespace Equinor.Procosys.Preservation.Query.ModeAggregate
{
    public class GetModeByIdQueryHandler : IRequestHandler<GetModeByIdQuery, ModeDto>
    {
        private readonly IModeRepository _modeRepository;

        public GetModeByIdQueryHandler(IModeRepository modeRepository) => _modeRepository = modeRepository;

        public async Task<ModeDto> Handle(GetModeByIdQuery request, CancellationToken cancellationToken)
        {
            var mode = await _modeRepository.GetByIdAsync(request.Id);
            return new ModeDto(mode.Id, mode.Title);
        }
    }
}
