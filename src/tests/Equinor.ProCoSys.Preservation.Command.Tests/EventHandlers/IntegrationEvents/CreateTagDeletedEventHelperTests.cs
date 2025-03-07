using System;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
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
        
        var requirementDefinition1Mock = new Mock<RequirementDefinition>();
        requirementDefinition1Mock.SetupGet(r => r.Plant).Returns(TestPlant);
        requirementDefinition1Mock.SetupGet(r => r.Id).Returns(1);
        var requirement1 = new TagRequirement(TestPlant, 1, requirementDefinition1Mock.Object);
        
        var requirementDefinition2Mock = new Mock<RequirementDefinition>();
        requirementDefinition2Mock.SetupGet(r => r.Plant).Returns(TestPlant);
        requirementDefinition2Mock.SetupGet(r => r.Id).Returns(2);
        var requirement2 = new TagRequirement(TestPlant, 2, requirementDefinition2Mock.Object);
        
        _tag = new Tag(
            TestPlant,
            TagType.Standard,
            Guid.NewGuid(),
            "Tag1",
            "",
            stepMock.Object,
            [requirement1, requirement2]);
        
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
    public async Task CreateEvents_ShouldTagDeleteEventExpectedValues(string property, object expected)
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
    public async Task CreateEvents_ShouldTagDeleteEventWithExpectedProCoSysGuid()
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
    public async Task CreateEvents_ShouldCreateIntegrationEventsForChildActions()
    {
        // Act
        var integrationEvents = await _dut.CreateEvents(_tag);

        // Assert
        Assert.AreEqual(2, integrationEvents.ActionDeleteEvents.Count());
    }
    
    [TestMethod]
    public async Task CreateEvents_ShouldCreateIntegrationEventsForChildTagRequirements()
    {
        // Act
        var integrationEvents = await _dut.CreateEvents(_tag);

        // Assert
        Assert.AreEqual(2, integrationEvents.TagRequirementDeleteEvents.Count());
    }
}
