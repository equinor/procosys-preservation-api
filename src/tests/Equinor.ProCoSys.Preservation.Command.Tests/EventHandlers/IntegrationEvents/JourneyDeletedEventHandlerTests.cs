using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
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
    private List<IIntegrationEvent> _publishedEvents;

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        var mockPublisher = new Mock<IIntegrationEventPublisher>();
        mockPublisher.Setup(x => x.PublishAsync(It.IsAny<IIntegrationEvent>(), default))
            .Callback<IIntegrationEvent, CancellationToken>((e, _) => _publishedEvents.Add(e));

        _publishedEvents = new List<IIntegrationEvent>();

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
        var types = _publishedEvents.Select(e => e.GetType()).ToList();

        // Assert
        CollectionAssert.Contains(types, typeof(JourneyDeleteEvent));
    }
    
    [TestMethod]
    public async Task Handle_ShouldSendStepDeleteEvent()
    {
        // Arrange
        var journey = new Journey(TestPlant, "Test Title");
        
        var mode = new Mode(TestPlant, "Test Title", true);
        var responsible = new Responsible(TestPlant, "C", "Test Description");
        var step = new Step(TestPlant, "Test Title 1", mode, responsible);
        journey.AddStep(step);
        
        var domainEvent = new DeletedEvent<Journey>(journey);

        // Act
        await _dut.Handle(domainEvent, default);
        var types = _publishedEvents.Select(e => e.GetType()).ToList();

        // Assert
        CollectionAssert.Contains(types, typeof(StepDeleteEvent));
    }
}
