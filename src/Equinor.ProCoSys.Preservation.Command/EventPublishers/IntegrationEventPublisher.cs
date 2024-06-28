using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.MessageContracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Equinor.ProCoSys.Preservation.Command.EventPublishers;

public class IntegrationEventPublisher : IIntegrationEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<IntegrationEventPublisher> _logger;

    public IntegrationEventPublisher(IPublishEndpoint publishEndpoint, ILogger<IntegrationEventPublisher> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken) where T : class, IIntegrationEvent
        => await _publishEndpoint.Publish(message,
            context =>
            {
                _logger.LogInformation("Publishing: {Message}", context.Message.ToString());
            },
            cancellationToken);

}
