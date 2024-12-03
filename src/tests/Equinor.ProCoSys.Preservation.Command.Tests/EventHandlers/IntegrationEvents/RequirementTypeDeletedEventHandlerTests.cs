using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
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
    private bool _eventPublished;

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        var mockPublisher = new Mock<IIntegrationEventPublisher>();
        mockPublisher.Setup(x => x.PublishAsync(It.IsAny<IIntegrationEvent>(), default))
            .Callback(() => _eventPublished = true);
        
        _eventPublished = false;

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

        // Assert
        Assert.IsTrue(_eventPublished);
    }
    
    [TestMethod]
    public async Task Handle_ShouldSendRequirementDefinitionDeleteEvent()
    {
        // Arrange
        var requirementType = new RequirementType(TestPlant, "Code", "Title", RequirementTypeIcon.Other, 10);
        
        var requirementDefinition = new RequirementDefinition(TestPlant, "Test Definition", 2, RequirementUsage.ForAll, 1);
        requirementType.AddRequirementDefinition(requirementDefinition);
        
        var domainEvent = new DeletedEvent<RequirementType>(requirementType);

        // Act
        await _dut.Handle(domainEvent, default);

        // Assert
        Assert.IsTrue(_eventPublished);
    }
    
    [TestMethod]
    public async Task Handle_ShouldSendFieldDeleteEvent()
    {
        // Arrange
        var requirementType = new RequirementType(TestPlant, "Code", "Title", RequirementTypeIcon.Other, 10);
        
        var requirementDefinition = new RequirementDefinition(TestPlant, "Test Definition", 2, RequirementUsage.ForAll, 1);
        requirementType.AddRequirementDefinition(requirementDefinition);
        
        var field = new Field(TestPlant, "Test Label", FieldType.Info, 0);
        requirementDefinition.AddField(field);
        
        var domainEvent = new DeletedEvent<RequirementType>(requirementType);

        // Act
        await _dut.Handle(domainEvent, default);

        // Assert
        Assert.IsTrue(_eventPublished);
    }
}
