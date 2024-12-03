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
public class RequirementDefinitionDeletedEventHandlerTests
{
    private const string TestPlant = "PCS$PlantA";
    private RequirementDefinitionDeletedEventHandler _dut;
    private List<IIntegrationEvent> _publishedEvents;

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        var mockPublisher = new Mock<IIntegrationEventPublisher>();
        mockPublisher.Setup(x => x.PublishAsync(It.IsAny<IIntegrationEvent>(), default))
            .Callback<IIntegrationEvent, CancellationToken>((e, _) => _publishedEvents.Add(e));

        _publishedEvents = new List<IIntegrationEvent>();

        _dut = new RequirementDefinitionDeletedEventHandler(mockPublisher.Object);
    }

    [TestMethod]
    public async Task Handle_ShouldSendRequirementDefinitionDelete()
    {
        // Arrange
        var requirementType = new RequirementDefinition(TestPlant, "Test Definition", 2, RequirementUsage.ForAll, 1);
        var domainEvent = new DeletedEvent<RequirementDefinition>(requirementType);

        // Act
        await _dut.Handle(domainEvent, default);
        var types = _publishedEvents.Select(e => e.GetType()).ToList();

        // Assert
        CollectionAssert.Contains(types, typeof(RequirementDefinitionDeleteEvent));
    }
    
    [TestMethod]
    public async Task Handle_ShouldSendFieldDeleteEvent()
    {
        // Arrange
        var requirementDefinition = new RequirementDefinition(TestPlant, "Test Definition", 2, RequirementUsage.ForAll, 1);
        
        var field = new Field(TestPlant, "Test Label", FieldType.Info, 0);
        requirementDefinition.AddField(field);
        
        var domainEvent = new DeletedEvent<RequirementDefinition>(requirementDefinition);

        // Act
        await _dut.Handle(domainEvent, default);
        var types = _publishedEvents.Select(e => e.GetType()).ToList();

        // Assert
        CollectionAssert.Contains(types, typeof(FieldDeleteEvent));
    }
}
