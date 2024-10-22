using System.Collections.Generic;
using System;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;
using System.Threading;
using Equinor.ProCoSys.Preservation.Command.Events;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.IntegrationEvents;

[TestClass]
public class PublishTagEventHelperTests
{
    private PublishTagEventHelper _dut;
    private IList<IIntegrationEvent> _publishedEvents;

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        var mockTagCreateEvent = new Mock<ICreateChildEventHelper<Project, Tag, TagEvent>>();
        mockTagCreateEvent.Setup(c => c.CreateEvent(It.IsAny<Project>(), It.IsAny<Tag>())).Returns(Task.FromResult(new TagEvent()));

        var mockProjectRepository = new Mock<IProjectRepository>();
        mockProjectRepository.Setup(x => x.GetProjectOnlyByTagGuidAsync(It.IsAny<Guid>())).ReturnsAsync(new Mock<Project>().Object);
        
        var createEventHelper = new Mock<ICreateChildEventHelper<Project, Tag, TagEvent>>();
        createEventHelper.Setup(c => c.CreateEvent(It.IsAny<Project>(), It.IsAny<Tag>())).Returns(Task.FromResult(new TagEvent()));
        
        _publishedEvents = new List<IIntegrationEvent>();
        
        var eventPublisher = new Mock<IIntegrationEventPublisher>();
        eventPublisher.Setup(x => x.PublishAsync(It.IsAny<IIntegrationEvent>(), default)).Callback<IIntegrationEvent, CancellationToken>((e, _) => _publishedEvents.Add(e));
        
        _dut = new PublishTagEventHelper(mockProjectRepository.Object, createEventHelper.Object, eventPublisher.Object);
    }

    [TestMethod]
    public async Task PublishEvent_ShouldPublishTagEvent()
    {
        // Arrange
        var tagMock = new Mock<Tag>();

        // Act
        await _dut.PublishEvent(tagMock.Object, default);
        var eventTypes = _publishedEvents.Select(e => e.GetType()).ToList();

        // Assert
        CollectionAssert.Contains(eventTypes, typeof(TagEvent));
    }
}
