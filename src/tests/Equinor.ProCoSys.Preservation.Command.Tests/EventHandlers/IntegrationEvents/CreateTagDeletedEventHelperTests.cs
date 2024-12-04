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
        
        var project = new Project(TestPlant, TestProjectName, "Test Project Description", Guid.NewGuid());
        project.AddTag(_tag);

        var mockProjectRepository = new Mock<IProjectRepository>();
        mockProjectRepository.Setup(x => x.GetProjectOnlyByTagGuidAsync(It.IsAny<Guid>())).ReturnsAsync(project);
        
        _dut = new CreateTagDeleteEventHelper(mockProjectRepository.Object);
    }

    [DataTestMethod]
    [DataRow(nameof(RequirementTypeDeleteEvent.Plant), TestPlant)]
    [DataRow(nameof(RequirementTypeDeleteEvent.ProjectName), TestProjectName)]
    [DataRow(nameof(RequirementTypeDeleteEvent.Behavior), "delete")]
    public async Task CreateEvent_ShouldCreateRequirementDefinitionEventExpectedValues(string property, object expected)
    {
        var deletionEvent = await _dut.CreateEvent(_tag);
        var result = deletionEvent.GetType()
            .GetProperties()
            .Single(p => p.Name == property)
            .GetValue(deletionEvent);

        // Assert
        Assert.AreEqual(expected, result);
    }
    
    [TestMethod]
    public async Task CreateEvent_ShouldCreateRequirementDefinitionEventWithExpectedProCoSysGuid()
    {
        // Arrange
        var expected = _tag.Guid;
        
        // Act
        var deletionEvent = await _dut.CreateEvent(_tag);
        var result = deletionEvent.ProCoSysGuid;

        // Assert
        Assert.AreEqual(expected, result);
    }
}
