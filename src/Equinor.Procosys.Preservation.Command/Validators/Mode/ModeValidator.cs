using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;

namespace Equinor.Procosys.Preservation.Command.Validators.Mode
{
    public class ModeValidator : IModeValidator
    {
        private readonly IModeRepository _modeRepository;
        private readonly IJourneyRepository _journeyRepository;

        public ModeValidator(IModeRepository modeRepository, IJourneyRepository journeyRepository)
        {
            _modeRepository = modeRepository;
            _journeyRepository = journeyRepository;
        }

        public bool Exists(int modeId)
            => _modeRepository.GetByIdAsync(modeId).Result != null;
        
        public bool Exists(string title)
            => _modeRepository.GetByTitleAsync(title).Result != null;

        public bool IsVoided(int modeId)
        {
            var mode = _modeRepository.GetByIdAsync(modeId).Result;
            return mode != null && mode.IsVoided;
        }

        public bool IsUsedInStep(int modeId)
        {
            var step = _journeyRepository.GetStepsByModeIdAsync(modeId).Result;
            return step != null && step.Count > 0;
        }
    }
}
