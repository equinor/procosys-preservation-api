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
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.IntegrationEvents;

[TestClass]
public class PublishRequirementDefinitionEventHelperTests
{
    private PublishRequirementDefinitionEventHelper _dut;
    private IList<IIntegrationEvent> _publishedEvents;

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        var mockRequirementTypeRepository = new Mock<IRequirementTypeRepository>();
        mockRequirementTypeRepository.Setup(x => x.GetRequirementTypeByRequirementDefinitionGuidAsync(It.IsAny<Guid>())).ReturnsAsync(new Mock<RequirementType>().Object);
        
        var createEventHelper = new Mock<ICreateChildEventHelper<RequirementType, RequirementDefinition, RequirementDefinitionEvent>>();
        createEventHelper.Setup(c => c.CreateEvent(It.IsAny<RequirementType>(), It.IsAny<RequirementDefinition>())).ReturnsAsync(new RequirementDefinitionEvent());
        
        _publishedEvents = new List<IIntegrationEvent>();
        
        var eventPublisher = new Mock<IIntegrationEventPublisher>();
        eventPublisher.Setup(x => x.PublishAsync(It.IsAny<IIntegrationEvent>(), default)).Callback<IIntegrationEvent, CancellationToken>((e, _) => _publishedEvents.Add(e));
        
        _dut = new PublishRequirementDefinitionEventHelper(mockRequirementTypeRepository.Object, createEventHelper.Object, eventPublisher.Object);
    }

    [TestMethod]
    public async Task PublishEvent_ShouldPublishTagEvent()
    {
        // Arrange
        var requirementDefinitionMock = new Mock<RequirementDefinition>();

        // Act
        await _dut.PublishEvent(requirementDefinitionMock.Object, default);
        var eventTypes = _publishedEvents.Select(e => e.GetType()).ToList();

        // Assert
        CollectionAssert.Contains(eventTypes, typeof(RequirementDefinitionEvent));
    }
}
