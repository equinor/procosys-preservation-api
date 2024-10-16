using System.Collections.Generic;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.Converters;

internal interface IDomainToIntegrationEventConverter<T> where T : IDomainEvent
{
    IEnumerable<IIntegrationEvent> Convert(T domainEvent);
}
