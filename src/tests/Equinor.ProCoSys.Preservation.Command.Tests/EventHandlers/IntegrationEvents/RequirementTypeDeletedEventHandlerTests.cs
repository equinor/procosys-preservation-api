using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Preservation.MessageContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.IntegrationEvents;

[TestClass]
public class RequirementTypeDeletedEventHandlerTests
{
    private const string TestPlant = "PCS$PlantA";
    private RequirementTypeDeletedEventHandler _dut;
    private List<IIntegrationEvent> _publishedEvents = new();

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        var mockPublisher = new Mock<IIntegrationEventPublisher>();
        mockPublisher.Setup(x => x.PublishAsync(It.IsAny<IIntegrationEvent>(), default))
            .Callback<IIntegrationEvent, CancellationToken>((e, _) => _publishedEvents.Add(e));

        _dut = new RequirementTypeDeletedEventHandler(mockPublisher.Object);
    }

    [TestMethod]
    public async Task Handle_ShouldSendRequirementTypeDeleteEvent()
    {
        // Arrange
        var requirementType = new RequirementType(TestPlant, "Code", "Title", RequirementTypeIcon.Other, 10);
        var domainEvent = new DeletedEvent<RequirementType>(requirementType);

        // Act
        await _dut.Handle(domainEvent, default);
        var types = _publishedEvents.Select(e => e.GetType()).ToList();

        // Assert
        CollectionAssert.Contains(types, typeof(RequirementTypeDeleteEvent));
    }
    
    [TestMethod]
    public async Task Handle_ShouldSendRequiredIntegrationEventsForChildRequirementDefinitions()
    {
        // Arrange
        var requirementType = new RequirementType(TestPlant, "Code", "Title", RequirementTypeIcon.Other, 10);
        
        var requirementDefinition1 = new RequirementDefinition(TestPlant, "Test Definition Title 1", 2, RequirementUsage.ForSuppliersOnly, 1);
        requirementType.AddRequirementDefinition(requirementDefinition1);
        var requirementDefinition2 = new RequirementDefinition(TestPlant, "Test Definition Title 2", 2, RequirementUsage.ForSuppliersOnly, 2);
        requirementType.AddRequirementDefinition(requirementDefinition2);
        
        var domainEvent = new DeletedEvent<RequirementType>(requirementType);

        // Act
        await _dut.Handle(domainEvent, default);
        var requirementDefinitionDeleteEventTypes = _publishedEvents.Select(e => e.GetType()).Where(e => e == typeof(RequirementDefinitionDeleteEvent)).ToList();

        // Assert
        Assert.AreEqual(2, requirementDefinitionDeleteEventTypes.Count);
    }
}
