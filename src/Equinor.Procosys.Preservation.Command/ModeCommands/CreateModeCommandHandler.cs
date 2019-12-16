using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using MediatR;

namespace Equinor.Procosys.Preservation.Command.ModeCommands
{
    public class CreateModeCommandHandler : IRequestHandler<CreateModeCommand, int>
    {
        private readonly IModeRepository _modeRepository;
        private readonly IPlantProvider _plantProvider;

        public CreateModeCommandHandler(IModeRepository modeRepository, IPlantProvider plantProvider)
        {
            _modeRepository = modeRepository;
            _plantProvider = plantProvider;
        }

        public async Task<int> Handle(CreateModeCommand request, CancellationToken cancellationToken)
        {
            var newMode = new Mode(_plantProvider.Plant, request.Title);
            _modeRepository.Add(newMode);
            await _modeRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newMode.Id;
        }
    }
}
