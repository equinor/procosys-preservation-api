using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;

namespace Equinor.Procosys.Preservation.Command.Validators.Step
{
    public class StepValidator : IStepValidator
    {
        private readonly IJourneyRepository _journeyRepository;

        public StepValidator(IJourneyRepository journeyRepository)
            => _journeyRepository = journeyRepository;

        public bool Exists(int stepId)
            => _journeyRepository.GetStepByStepIdAsync(stepId).Result != null;

        public bool IsVoided(int stepId)
        {
            var r = _journeyRepository.GetStepByStepIdAsync(stepId).Result;
            return r != null && r.IsVoided;
        }
    }
}
