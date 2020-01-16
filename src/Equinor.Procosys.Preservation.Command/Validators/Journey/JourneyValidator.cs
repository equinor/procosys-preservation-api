using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;

namespace Equinor.Procosys.Preservation.Command.Validators.Journey
{
    public class JourneyValidator : IJourneyValidator
    {
        private readonly IJourneyRepository _journeyRepository;

        public JourneyValidator(IJourneyRepository journeyRepository)
            => _journeyRepository = journeyRepository;

        public bool Exists(int journeyId)
            => _journeyRepository.GetByIdAsync(journeyId).Result != null;

        public bool Exists(string title)
            => _journeyRepository.GetByTitleAsync(title).Result != null;

        public bool IsVoided(int journeyId) => throw new System.NotImplementedException("journey.isvoided"); // todo
    }
}
