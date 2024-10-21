using System.Collections.Generic;
using System;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.Converters;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Moq;
using Equinor.ProCoSys.Preservation.Domain.Events;
using System.Linq;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Common.Time;
using Equinor.ProCoSys.Preservation.Test.Common;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.IntegrationEvents;

[TestClass]
public class ProjectTagAddedEventConverterTests
{
    private const string TestPlant = "PCS$PlantA";
    private Project _project;
    private Tag _tag;
    private ProjectTagAddedEventConverter _dut;

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        var requirementDefinition = new RequirementDefinition(TestPlant, "D2", 2, RequirementUsage.ForSuppliersOnly, 1);
        var tagRequirement = new TagRequirement(TestPlant, 2, requirementDefinition);

        var stepMock = new Mock<Step>();
        stepMock.SetupGet(s => s.Plant).Returns(TestPlant);
        _tag = new Tag(TestPlant, TagType.Standard, Guid.NewGuid(), "Tag1", "", stepMock.Object, new List<TagRequirement> { tagRequirement });

        _project = new Project(TestPlant, "Test Project", "Test Project Description", Guid.NewGuid());
        _project.AddTag(_tag);

        var mockTagCreateEvent = new Mock<ICreateProjectEventHelper<Tag, TagEvent>>();
        mockTagCreateEvent.Setup(c => c.CreateEvent(It.IsAny<Tag>(), It.IsAny<string>())).Returns(Task.FromResult(new TagEvent()));
        
        var mockTagRequirementCreateEvent = new Mock<ICreateProjectEventHelper<TagRequirement, TagRequirementEvent>>();
        mockTagRequirementCreateEvent.Setup(c => c.CreateEvent(It.IsAny<TagRequirement>(), It.IsAny<string>())).Returns(Task.FromResult(new TagRequirementEvent()));

        _dut = new ProjectTagAddedEventConverter(mockTagCreateEvent.Object, mockTagRequirementCreateEvent.Object);
    }

    [TestMethod]
    public async Task Convert_ShouldConvertToIntegrationEventsWithTagEvent()
    {
        // Arrange
        var domainEvent = new ProjectTagAddedEvent(_project, _tag);

        // Act
        var integrationEvents = await _dut.Convert(domainEvent);
        var eventTypes = integrationEvents.Select(e => e.GetType()).ToList();

        // Assert
        CollectionAssert.Contains(eventTypes, typeof(TagEvent));
    }
    
    [TestMethod]
    public async Task Convert_ShouldConvertToIntegrationEventsWithTagRequirementEvent()
    {
        // Arrange
        var domainEvent = new ProjectTagAddedEvent(_project, _tag);

        // Act
        var integrationEvents = await _dut.Convert(domainEvent);
        var eventTypes = integrationEvents.Select(e => e.GetType()).ToList();

        // Assert
        CollectionAssert.Contains(eventTypes, typeof(TagRequirementEvent));
    }
}
