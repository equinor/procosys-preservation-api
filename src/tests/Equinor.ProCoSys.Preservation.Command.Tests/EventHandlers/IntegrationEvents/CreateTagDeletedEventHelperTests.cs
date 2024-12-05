using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.IntegrationEvents;

[TestClass]
public class CreateTagDeletedEventHelperTests
{
    private const string TestPlant = "PCS$PlantA";
    private const string TestProjectName = "Test Project";
    private CreateTagDeleteEventHelper _dut;
    private Tag _tag;

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        var stepMock = new Mock<Step>();
        stepMock.SetupGet(s => s.Plant).Returns(TestPlant);
        
        var tagRequirementMock = new Mock<TagRequirement>();
        tagRequirementMock.SetupGet(r => r.Plant).Returns(TestPlant);
        _tag = new Tag(TestPlant, TagType.Standard, Guid.NewGuid(), "Tag1", "", stepMock.Object,
            new List<TagRequirement> { tagRequirementMock.Object });
        
        var action1 = new Action(TestPlant, "Test Action 1", "Test Action Description", null);
        _tag.AddAction(action1);
        
        var action2 = new Action(TestPlant, "Test Action 2", "Test Action Description", null);
        _tag.AddAction(action2);
        
        var project = new Project(TestPlant, TestProjectName, "Test Project Description", Guid.NewGuid());
        project.AddTag(_tag);

        var mockProjectRepository = new Mock<IProjectRepository>();
        mockProjectRepository.Setup(x => x.GetProjectOnlyByTagGuidAsync(It.IsAny<Guid>())).ReturnsAsync(project);
        
        _dut = new CreateTagDeleteEventHelper(mockProjectRepository.Object);
    }

    [DataTestMethod]
    [DataRow(nameof(TagDeleteEvent.Plant), TestPlant)]
    [DataRow(nameof(TagDeleteEvent.ProjectName), TestProjectName)]
    [DataRow(nameof(TagDeleteEvent.Behavior), "delete")]
    public async Task CreateEvent_ShouldTagDeleteEventExpectedValues(string property, object expected)
    {
        // Act
        var deletionEvents = await _dut.CreateEvents(_tag);
        var tagDeleteEvent = deletionEvents.TagDeleteEvent;
        var result = tagDeleteEvent.GetType()
            .GetProperties()
            .Single(p => p.Name == property)
            .GetValue(tagDeleteEvent);

        // Assert
        Assert.AreEqual(expected, result);
    }
    
    [TestMethod]
    public async Task CreateEvent_ShouldTagDeleteEventWithExpectedProCoSysGuid()
    {
        // Arrange
        var expected = _tag.Guid;
        
        // Act
        var deletionEvent = await _dut.CreateEvents(_tag);
        var result = deletionEvent.TagDeleteEvent.ProCoSysGuid;

        // Assert
        Assert.AreEqual(expected, result);
    }
    
    [TestMethod]
    public async Task CreateEvent_ShouldCreateIntegrationEventsForChildActions()
    {
        // Act
        var integrationEvents = await _dut.CreateEvents(_tag);

        // Assert
        Assert.AreEqual(2, integrationEvents.ActionDeleteEvents.Count());
    }
}
