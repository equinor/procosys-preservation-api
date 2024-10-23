using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Preservation.MessageContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.IntegrationEvents;

[TestClass]
public class TagActionAddedEventHandlerTests
{
    private const string TestPlant = "PCS$PlantA";
    private TagActionAddedEventHandler _dut;
    private bool _eventPublished;
    private Tag _tag;
    private Action _action;

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        var mockCreateEventHelper = new Mock<ICreateChildEventHelper<Tag, Action, ActionEvent>>();
        mockCreateEventHelper.Setup(x => x.CreateEvent(It.IsAny<Tag>(), It.IsAny<Action>())).ReturnsAsync(new ActionEvent());

        var mockPublisher = new Mock<IIntegrationEventPublisher>();
        mockPublisher.Setup(x => x.PublishAsync(It.IsAny<IIntegrationEvent>(), default))
            .Callback(() => _eventPublished = true);

        _eventPublished = false;

        _dut = new TagActionAddedEventHandler(mockCreateEventHelper.Object, mockPublisher.Object);

        var requirementDefinition = new RequirementDefinition(TestPlant, "D2", 2, RequirementUsage.ForSuppliersOnly, 1);
        var tagRequirement= new TagRequirement(TestPlant, 2, requirementDefinition);

        var stepMock = new Mock<Step>();
        stepMock.SetupGet(s => s.Plant).Returns(TestPlant);
        _tag = new Tag(TestPlant, TagType.Standard, Guid.NewGuid(), "Tag1", "", stepMock.Object, new List<TagRequirement> { tagRequirement });

        _action = new Action(TestPlant, "Test Action", "Test Action Description", null);
        _tag.AddAction(_action);
    }

    [TestMethod]
    public async Task Handle_ShouldSendIntegrationEvent()
    {
        // Arrange
        var domainEvent = new EntityAddedChildEntityEvent<Tag, Action>(_tag, _action);

        // Act
        await _dut.Handle(domainEvent, default);

        // Assert
        Assert.IsTrue(_eventPublished);
    }
}
