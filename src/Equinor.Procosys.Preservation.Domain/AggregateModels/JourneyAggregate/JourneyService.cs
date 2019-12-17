using System;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.Exceptions;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate
{
    public class JourneyService : IJourneyService
    {
        private readonly IJourneyRepository _repository;

        public JourneyService(IJourneyRepository repository) => _repository = repository ?? throw new ArgumentNullException(nameof(repository));

        public async Task ValidateNewJourney(Journey journey)
        {
            var existingJourney = await _repository.GetByTitleAsync(journey.Title);
            if (existingJourney != null)
            {
                throw new ProcosysEntityValidationException($"{nameof(Journey)} {nameof(Journey.Title)} must be unique");
            }
        }
    }
}
