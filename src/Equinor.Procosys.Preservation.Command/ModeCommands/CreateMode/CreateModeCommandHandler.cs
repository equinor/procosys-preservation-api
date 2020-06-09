using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.ModeCommands.CreateMode
{
    public class CreateModeCommandHandler : IRequestHandler<CreateModeCommand, Result<int>>
    {
        private readonly IModeRepository _modeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlantProvider _plantProvider;

        public CreateModeCommandHandler(IModeRepository modeRepository, IUnitOfWork unitOfWork, IPlantProvider plantProvider)
        {
            _modeRepository = modeRepository;
            _unitOfWork = unitOfWork;
            _plantProvider = plantProvider;
        }

        public async Task<Result<int>> Handle(CreateModeCommand request, CancellationToken cancellationToken)
        {
            var newMode = new Mode(_plantProvider.Plant, request.Title, request.ForSupplier);
            _modeRepository.Add(newMode);
            await _unitOfWork.SaveChangesAsync(request.CurrentUserOid, cancellationToken);
            return new SuccessResult<int>(newMode.Id);
        }
    }
}
