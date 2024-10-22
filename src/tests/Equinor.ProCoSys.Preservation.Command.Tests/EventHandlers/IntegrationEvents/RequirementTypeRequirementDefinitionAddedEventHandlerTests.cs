using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.Converters;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Preservation.MessageContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.IntegrationEvents;

[TestClass]
public class RequirementTypeRequirementDefinitionAddedEventHandlerTests
{
    private const string TestPlant = "PCS$PlantA";
    private RequirementTypeRequirementDefinitionAddedEventHandler _dut;
    private bool _eventPublished;
    private RequirementType _requirementType;
    private RequirementDefinition _requirementDefinition;

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        var mockIntegrationEvent = new Mock<IIntegrationEvent>();
        var mockConverter = new Mock<IDomainToIntegrationEventConverter<RequirementTypeRequirementDefinitionAddedEvent>>();
        mockConverter.Setup(x => x.Convert(It.IsAny<RequirementTypeRequirementDefinitionAddedEvent>())).ReturnsAsync(new List<IIntegrationEvent>() { mockIntegrationEvent.Object });

        var mockPublisher = new Mock<IIntegrationEventPublisher>();
        mockPublisher.Setup(x => x.PublishAsync(It.IsAny<IIntegrationEvent>(), default))
            .Callback(() => _eventPublished = true);

        _eventPublished = false;

        _dut = new RequirementTypeRequirementDefinitionAddedEventHandler(mockConverter.Object, mockPublisher.Object);

        _requirementType = new RequirementType(TestPlant, "CodeA", "TitleA", RequirementTypeIcon.Other, 10);
        _requirementDefinition = new RequirementDefinition(TestPlant, "D2", 2, RequirementUsage.ForSuppliersOnly, 1);

    }

    [TestMethod]
    public async Task Handle_ShouldSendIntegrationEvent()
    {
        // Arrange
        var domainEvent = new RequirementTypeRequirementDefinitionAddedEvent(_requirementType, _requirementDefinition);

        // Act
        await _dut.Handle(domainEvent, default);

        // Assert
        Assert.IsTrue(_eventPublished);
    }
}
