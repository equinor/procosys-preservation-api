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
public class TagRequirementDeletedEventHandlerTests
{
    private TagRequirementDeletedEventHandler _dut;
    private IList<IIntegrationEvent> _publishedEvents;

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        var mockPublisher = new Mock<IIntegrationEventPublisher>();
        mockPublisher.Setup(x => x.PublishAsync(It.IsAny<IIntegrationEvent>(), default))
            .Callback<IIntegrationEvent, CancellationToken>((e, _) => _publishedEvents.Add(e));
        
        var mockProjectRepository = new Mock<IProjectRepository>();
        mockProjectRepository.Setup(p => p.GetProjectOnlyByTagGuidAsync(It.IsAny<Guid>())).ReturnsAsync(new Mock<Project>().Object);
        
        _dut = new TagRequirementDeletedEventHandler(mockPublisher.Object, mockProjectRepository.Object);
        
        _publishedEvents = [];
    }

    [TestMethod]
    public async Task Handle_ShouldSendTagDeleteEvent()
    {
        // Arrange
        var tagRequirement = new Mock<TagRequirement>();
        var domainEvent = new TagRequirementDeletedEvent(string.Empty, Guid.Empty, tagRequirement.Object);

        // Act
        await _dut.Handle(domainEvent, CancellationToken.None);
        var types = _publishedEvents.Select(e => e.GetType()).ToList();

        // Assert
        CollectionAssert.Contains(types, typeof(TagRequirementDeleteEvent));
    }
}
