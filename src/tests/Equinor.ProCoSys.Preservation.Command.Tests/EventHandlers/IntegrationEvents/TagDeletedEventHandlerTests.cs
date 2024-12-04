using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Preservation.MessageContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.IntegrationEvents;

[TestClass]
public class TagDeletedEventHandlerTests
{
    private const string TestPlant = "PCS$PlantA";
    private TagDeletedEventHandler _dut;
    private IIntegrationEvent _publishedEvent;

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        
        var mockCreateEventHelper = new Mock<ICreateEventHelper<Tag, TagDeleteEvent>>();
        var emptyDeleteEvent = new TagDeleteEvent(Guid.Empty, string.Empty, string.Empty);
        mockCreateEventHelper.Setup(x => x.CreateEvent(It.IsAny<Tag>())).ReturnsAsync(emptyDeleteEvent);
        
        var mockPublisher = new Mock<IIntegrationEventPublisher>();
        mockPublisher.Setup(x => x.PublishAsync(It.IsAny<IIntegrationEvent>(), default))
            .Callback<IIntegrationEvent, CancellationToken>((e, _) => _publishedEvent = e);

        _publishedEvent = null;

        _dut = new TagDeletedEventHandler(mockCreateEventHelper.Object, mockPublisher.Object);
    }

    [TestMethod]
    public async Task Handle_ShouldSendIntegrationEvent()
    {
        // Arrange
        var stepMock = new Mock<Step>();
        stepMock.SetupGet(s => s.Plant).Returns(TestPlant);
        
        var tagRequirementMock = new Mock<TagRequirement>();
        tagRequirementMock.SetupGet(r => r.Plant).Returns(TestPlant);
        var tag = new Tag(TestPlant, TagType.Standard, Guid.NewGuid(), "Tag1", "", stepMock.Object,
            new List<TagRequirement> { tagRequirementMock.Object });
        
        var domainEvent = new DeletedEvent<Tag>(tag);

        // Act
        await _dut.Handle(domainEvent, default);

        // Assert
        Assert.IsInstanceOfType<TagDeleteEvent>(_publishedEvent);
    }
}
