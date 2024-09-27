using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public class CreatePreservationPeriodDeletedEventHelper  : ICreateEventHelper<PreservationPeriod, PreservationPeriodDeleteEvent>
{
    public Task<PreservationPeriodDeleteEvent> CreateEvent(PreservationPeriod entity) => Task.FromResult(new PreservationPeriodDeleteEvent(entity.Guid, entity.Plant));
}
