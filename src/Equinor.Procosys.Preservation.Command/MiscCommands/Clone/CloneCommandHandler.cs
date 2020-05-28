using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.MiscCommands.Clone
{
    public class CloneCommandHandler : IRequestHandler<CloneCommand, Result<Unit>>
    {
        private readonly IPlantProvider _plantProvider;
        private readonly IModeRepository _modeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CloneCommandHandler(IPlantProvider plantProvider, IModeRepository modeRepository, IUnitOfWork unitOfWork)
        {
            _plantProvider = plantProvider;
            _modeRepository = modeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Unit>> Handle(CloneCommand request, CancellationToken cancellationToken)
        {
            var targetPlant = _plantProvider.Plant;

            if (targetPlant != request.TargetPlant)
            {
                throw new Exception($"Target plant '{request.TargetPlant}' in request must match target plant '{targetPlant}' in provider");
            }

            await CloneModes(request.SourcePlant, targetPlant);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<Unit>(Unit.Value);
        }

        private async Task CloneModes(string sourcePlant, string targetPlant)
        {
            _plantProvider.SetTemporaryPlant(sourcePlant);
            var sourceModes = await _modeRepository.GetAllAsync();
            _plantProvider.ReleaseTemporaryPlant();

            var targetModes = await _modeRepository.GetAllAsync();

            foreach (var sourceMode in sourceModes)
            {
                if (targetModes.SingleOrDefault(t => t.Title == sourceMode.Title) == null)
                {
                    var targetMode = new Mode(targetPlant, sourceMode.Title);
                    _modeRepository.Add(targetMode);
                }
            }
        }
    }
}
