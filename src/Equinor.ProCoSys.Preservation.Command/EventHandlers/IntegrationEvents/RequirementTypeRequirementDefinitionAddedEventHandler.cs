using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.Converters;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class RequirementTypeRequirementDefinitionAddedEventHandler : INotificationHandler<RequirementTypeRequirementDefinitionAddedEvent>
{
    private readonly IDomainToIntegrationEventConverter<RequirementTypeRequirementDefinitionAddedEvent> _converter;
    private readonly IIntegrationEventPublisher _integrationEventPublisher;

    public RequirementTypeRequirementDefinitionAddedEventHandler(IDomainToIntegrationEventConverter<RequirementTypeRequirementDefinitionAddedEvent> converter, IIntegrationEventPublisher integrationEventPublisher)
    {
        _converter = converter;
        _integrationEventPublisher = integrationEventPublisher;
    }

    public async Task Handle(RequirementTypeRequirementDefinitionAddedEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvents = await _converter.Convert(notification);
        foreach (var integrationEvent in integrationEvents)
        {
            await _integrationEventPublisher.PublishAsync(integrationEvent, cancellationToken);
        }
    }
}
