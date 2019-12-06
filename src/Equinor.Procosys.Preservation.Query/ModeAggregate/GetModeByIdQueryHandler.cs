using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.Exceptions;
using MediatR;

namespace Equinor.Procosys.Preservation.Query.ModeAggregate
{
    public class GetModeByIdQueryHandler : IRequestHandler<GetModeByIdQuery, ModeDto>
    {
        private readonly IModeRepository _modeRepository;

        public GetModeByIdQueryHandler(IModeRepository modeRepository)
        {
            _modeRepository = modeRepository;
        }

        public async Task<ModeDto> Handle(GetModeByIdQuery request, CancellationToken cancellationToken)
        {
            Mode mode = await _modeRepository.GetByIdAsync(request.Id);
            if (mode == null)
                throw new ProcosysEntityNotFoundException($"{nameof(Mode)} with ID {request.Id} not found");
            return new ModeDto(mode.Id, mode.Title);
        }
    }
}
