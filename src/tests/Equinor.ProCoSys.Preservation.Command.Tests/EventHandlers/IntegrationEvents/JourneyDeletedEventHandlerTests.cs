using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Preservation.MessageContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.IntegrationEvents;

[TestClass]
public class JourneyDeletedEventHandlerTests
{
    private const string TestPlant = "PCS$PlantA";
    private JourneyDeletedEventHandler _dut;
    private IIntegrationEvent _publishedEvent;

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        var mockPublisher = new Mock<IIntegrationEventPublisher>();
        mockPublisher.Setup(x => x.PublishAsync(It.IsAny<IIntegrationEvent>(), default))
            .Callback<IIntegrationEvent, CancellationToken>((e, _) => _publishedEvent = e);

        _publishedEvent = null;

        _dut = new JourneyDeletedEventHandler(mockPublisher.Object);
    }

    [TestMethod]
    public async Task Handle_ShouldSendIntegrationEvent()
    {
        // Arrange
        var journey = new Journey(TestPlant, "Test Title");
        var domainEvent = new DeletedEvent<Journey>(journey);

        // Act
        await _dut.Handle(domainEvent, default);

        // Assert
        Assert.IsInstanceOfType<FieldDeleteEvent>(_publishedEvent);
    }
}
