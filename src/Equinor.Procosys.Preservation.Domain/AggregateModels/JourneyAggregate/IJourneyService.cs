using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate
{
    /// <summary>
    /// Logic that spans multiple aggregates
    /// </summary>
    public interface IJourneyService
    {
        /// <summary>
        /// Checks if a new journey is valid with respect to other journeys
        /// </summary>
        /// <param name="journey"></param>
        /// <returns></returns>
        Task ValidateNewJourney(Journey journey);
    }
}
