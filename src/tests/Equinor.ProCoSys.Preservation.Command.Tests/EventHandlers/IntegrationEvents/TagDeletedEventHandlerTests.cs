using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Preservation.MessageContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.IntegrationEvents;

[TestClass]
public class TagDeletedEventHandlerTests
{
    private TagDeletedEventHandler _dut;
    private List<IIntegrationEvent> _publishedEvents;

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        var emptyTagDeleteEvent = new TagDeleteEvent(Guid.Empty, string.Empty, string.Empty);
        var emptyActionDeleteEvent = new ActionDeleteEvent(Guid.Empty, string.Empty, string.Empty);
        var emptyTagRequirementDeleteEvent = new TagRequirementDeleteEvent(Guid.Empty, string.Empty, string.Empty);
        var emptyTagRequirementDeleteEvents = new TagRequirementDeleteEvents(emptyTagRequirementDeleteEvent, []);
        var deleteEvents = new TagDeleteEvents(
            emptyTagDeleteEvent,
            [emptyActionDeleteEvent],
            [emptyTagRequirementDeleteEvents]);
        
        var mockCreateEventHelper = new Mock<ICreateTagDeleteEventHelper>();
        mockCreateEventHelper.Setup(x => x.CreateEvents(It.IsAny<Tag>())).ReturnsAsync(deleteEvents);
        
        var mockPublisher = new Mock<IIntegrationEventPublisher>();
        mockPublisher.Setup(x => x.PublishAsync(It.IsAny<IIntegrationEvent>(), default))
            .Callback<IIntegrationEvent, CancellationToken>((e, _) => _publishedEvents.Add(e));

        _publishedEvents = new List<IIntegrationEvent>();

        _dut = new TagDeletedEventHandler(mockCreateEventHelper.Object, mockPublisher.Object);
    }

    [TestMethod]
    public async Task Handle_ShouldSendTagDeleteEvent()
    {
        // Arrange
        var tag = new Mock<Tag>();
        var domainEvent = new DeletedEvent<Tag>(tag.Object);

        // Act
        await _dut.Handle(domainEvent, CancellationToken.None);
        var types = _publishedEvents.Select(e => e.GetType()).ToList();

        // Assert
        CollectionAssert.Contains(types, typeof(TagDeleteEvent));
    }
    
    [TestMethod]
    public async Task Handle_ShouldSendActionDeleteEvent()
    {
        // Arrange
        var tag = new Mock<Tag>();
        var domainEvent = new DeletedEvent<Tag>(tag.Object);

        // Act
        await _dut.Handle(domainEvent, CancellationToken.None);
        var types = _publishedEvents.Select(e => e.GetType()).ToList();

        // Assert
        CollectionAssert.Contains(types, typeof(ActionDeleteEvent));
    }
    
    [TestMethod]
    public async Task Handle_ShouldTagRequirementDeleteEvent()
    {
        // Arrange
        var tag = new Mock<Tag>();
        var domainEvent = new DeletedEvent<Tag>(tag.Object);

        // Act
        await _dut.Handle(domainEvent, CancellationToken.None);
        var types = _publishedEvents.Select(e => e.GetType()).ToList();

        // Assert
        CollectionAssert.Contains(types, typeof(TagRequirementDeleteEvent));
    }
}
