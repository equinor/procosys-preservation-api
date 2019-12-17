using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.Exceptions;
using MediatR;

namespace Equinor.Procosys.Preservation.Command.ModeCommands
{
    public class DeleteModeCommandHandler : IRequestHandler<DeleteModeCommand, Unit>
    {
        private readonly IModeRepository _modeRepository;

        public DeleteModeCommandHandler(IModeRepository modeRepository) => _modeRepository = modeRepository;

        public async Task<Unit> Handle(DeleteModeCommand request, CancellationToken cancellationToken)
        {
            var mode = await _modeRepository.GetByIdAsync(request.ModeId) ?? throw new ProcosysEntityNotFoundException($"{nameof(Mode)} with ID not found");
            _modeRepository.Remove(mode);
            await _modeRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
