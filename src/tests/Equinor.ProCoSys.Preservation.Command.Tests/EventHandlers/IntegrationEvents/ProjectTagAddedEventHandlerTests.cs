using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.Converters;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Preservation.MessageContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.IntegrationEvents;

[TestClass]
public class ProjectTagAddedEventHandlerTests
{
    private const string TestPlant = "PCS$PlantA";
    private ProjectTagAddedEventHandler _dut;
    private bool _eventPublished;
    private Tag _tag;
    private Project _project;

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        var mockIntegrationEvent = new Mock<IIntegrationEvent>();
        var mockConverter = new Mock<IDomainToIntegrationEventConverter<ProjectTagAddedEvent>>();
        mockConverter.Setup(x => x.Convert(It.IsAny<ProjectTagAddedEvent>())).ReturnsAsync(new List<IIntegrationEvent>() { mockIntegrationEvent.Object });

        var mockPublisher = new Mock<IIntegrationEventPublisher>();
        mockPublisher.Setup(x => x.PublishAsync(It.IsAny<IIntegrationEvent>(), default))
            .Callback(() => _eventPublished = true);

        _eventPublished = false;

        _dut = new ProjectTagAddedEventHandler(mockConverter.Object, mockPublisher.Object);

        var requirementDefinition = new RequirementDefinition(TestPlant, "D2", 2, RequirementUsage.ForSuppliersOnly, 1);
        var tagRequirement= new TagRequirement(TestPlant, 2, requirementDefinition);

        var stepMock = new Mock<Step>();
        stepMock.SetupGet(s => s.Plant).Returns(TestPlant);
        _tag = new Tag(TestPlant, TagType.Standard, Guid.NewGuid(), "Tag1", "", stepMock.Object, new List<TagRequirement> { tagRequirement });

        _project = new Project(TestPlant, "Test Project", "Test Project Description", Guid.NewGuid());
        _project.AddTag(_tag);
    }

    [TestMethod]
    public async Task Handle_ShouldSendIntegrationEvent()
    {
        // Arrange
        var domainEvent = new ProjectTagAddedEvent(_project, _tag);

        // Act
        await _dut.Handle(domainEvent, default);

        // Assert
        Assert.IsTrue(_eventPublished);
    }
}
