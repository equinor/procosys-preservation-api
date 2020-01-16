using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;

namespace Equinor.Procosys.Preservation.Command.Validators.Mode
{
    public class ModeValidator : IModeValidator
    {
        private readonly IModeRepository _modeRepository;

        public ModeValidator(IModeRepository modeRepository)
            => _modeRepository = modeRepository;

        public bool Exists(int modeId)
            => _modeRepository.GetByIdAsync(modeId).Result != null;

        public bool IsVoided(int modeId) => throw new System.NotImplementedException("mode.isvoided"); // todo
    }
}
