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
public class PublishTagRequirementEventHelperTests
{
    private PublishTagRequirementEventHelper _dut;
    private IList<IIntegrationEvent> _publishedEvents;

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        var mockTagRequirementCreateEvent = new Mock<ICreateProjectEventHelper<TagRequirement, TagRequirementEvent>>();
        mockTagRequirementCreateEvent.Setup(c => c.CreateEvent(It.IsAny<TagRequirement>(), It.IsAny<string>())).Returns(Task.FromResult(new TagRequirementEvent()));

        var mockProjectRepository = new Mock<IProjectRepository>();
        mockProjectRepository.Setup(x => x.GetTagByTagRequirementGuidAsync(It.IsAny<Guid>())).ReturnsAsync(new Mock<Tag>().Object);
        mockProjectRepository.Setup(x => x.GetProjectOnlyByTagGuidAsync(It.IsAny<Guid>())).ReturnsAsync(new Mock<Project>().Object);
        
        var createEventHelper = new Mock<ICreateProjectEventHelper<TagRequirement, TagRequirementEvent>>();
        createEventHelper.Setup(c => c.CreateEvent(It.IsAny<TagRequirement>(), It.IsAny<string>())).Returns(Task.FromResult(new TagRequirementEvent()));
        
        _publishedEvents = new List<IIntegrationEvent>();
        
        var eventPublisher = new Mock<IIntegrationEventPublisher>();
        eventPublisher.Setup(x => x.PublishAsync(It.IsAny<IIntegrationEvent>(), default)).Callback<IIntegrationEvent, CancellationToken>((e, _) => _publishedEvents.Add(e));
        
        _dut = new PublishTagRequirementEventHelper(mockProjectRepository.Object, createEventHelper.Object, eventPublisher.Object);
    }

    [TestMethod]
    public async Task PublishEvent_ShouldPublishTagRequirementEvent()
    {
        // Arrange
        var requirementMock = new Mock<TagRequirement>();

        // Act
        await _dut.PublishEvent(requirementMock.Object, default);
        var eventTypes = _publishedEvents.Select(e => e.GetType()).ToList();

        // Assert
        CollectionAssert.Contains(eventTypes, typeof(TagRequirementEvent));
    }
}
