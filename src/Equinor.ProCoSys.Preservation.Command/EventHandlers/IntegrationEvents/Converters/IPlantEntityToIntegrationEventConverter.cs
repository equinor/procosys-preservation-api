using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.Converters;

internal interface IDomainToIntegrationEventConverter<T> where T : IDomainEvent
{
    Task<IEnumerable<IIntegrationEvent>> Convert(T domainEvent);
}
