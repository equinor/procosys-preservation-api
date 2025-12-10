using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.ModeCommands.DeleteMode
{
    public class DeleteModeCommandHandler : IRequestHandler<DeleteModeCommand, Result<Unit>>
    {
        private readonly IModeRepository _modeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteModeCommandHandler(IModeRepository modeRepository, IUnitOfWork unitOfWork)
        {
            _modeRepository = modeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Unit>> Handle(DeleteModeCommand request, CancellationToken cancellationToken)
        {
            var mode = await _modeRepository.GetByIdAsync(request.ModeId);

            mode.SetRowVersion(request.RowVersion);
            _modeRepository.Remove(mode);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            mode.SetRemoved();
            return new SuccessResult<Unit>(Unit.Value);
        }
    }
}
