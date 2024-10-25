using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Preservation.MessageContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.IntegrationEvents;

[TestClass]
public class EntityAddedChildEntityEventHandlerTests
{
    private const string TestPlant = "PCS$PlantA";
    private EntityAddedChildEntityEventHandler<RequirementDefinition, Field, FieldEvent> _dut;
    private bool _eventPublished;
    private RequirementDefinition _requirementDefinition;
    private Field _field;

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        var mockCreateEventHelper = new Mock<ICreateChildEventHelper<RequirementDefinition, Field, FieldEvent>>();
        mockCreateEventHelper.Setup(x => x.CreateEvent(It.IsAny<RequirementDefinition>(), It.IsAny<Field>())).ReturnsAsync(new FieldEvent());

        var mockPublisher = new Mock<IIntegrationEventPublisher>();
        mockPublisher.Setup(x => x.PublishAsync(It.IsAny<IIntegrationEvent>(), default))
            .Callback(() => _eventPublished = true);

        _eventPublished = false;

        _dut = new EntityAddedChildEntityEventHandler<RequirementDefinition, Field, FieldEvent>(mockCreateEventHelper.Object, mockPublisher.Object);

        _requirementDefinition = new RequirementDefinition(TestPlant, "D2", 2, RequirementUsage.ForSuppliersOnly, 1);
        _field = new Field(TestPlant, "F1", FieldType.Number, 1, "UnitA", true);
    }

    [TestMethod]
    public async Task Handle_ShouldSendIntegrationEvent()
    {
        // Arrange
        var domainEvent = new EntityAddedChildEntityEvent<RequirementDefinition, Field>(_requirementDefinition, _field);

        // Act
        await _dut.Handle(domainEvent, default);

        // Assert
        Assert.IsTrue(_eventPublished);
    }
}
