using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Equinor.ProCoSys.Preservation.Command.Events;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.IntegrationEvents;

[TestClass]
public class CreateTagRequirementEventHelperTests
{
    private CreateTagRequirementEventHelper _dut;

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        var mockProjectRepository = new Mock<IProjectRepository>();
        mockProjectRepository.Setup(x => x.GetTagByTagRequirementGuidAsync(It.IsAny<Guid>())).ReturnsAsync(new Mock<Tag>().Object);
        mockProjectRepository.Setup(x => x.GetProjectOnlyByTagGuidAsync(It.IsAny<Guid>())).ReturnsAsync(new Mock<Project>().Object);
        
        var createEventHelper = new Mock<ICreateChildEventHelper<Project, TagRequirement, TagRequirementEvent>>();
        createEventHelper.Setup(c => c.CreateEvent(It.IsAny<Project>(), It.IsAny<TagRequirement>())).ReturnsAsync(new TagRequirementEvent());
        
        _dut = new CreateTagRequirementEventHelper(mockProjectRepository.Object, createEventHelper.Object);
    }

    [TestMethod]
    public async Task CreateEvent_ShouldCreateEvent()
    {
        // Arrange
        var requirementDefinition = new RequirementDefinition("PCS$PlantA", "D2", 2, RequirementUsage.ForSuppliersOnly, 1);
        var tagRequirement = new TagRequirement("PCS$PlantA", 2, requirementDefinition);
        
        // Act
        var integrationEvent = await _dut.CreateEvent(tagRequirement);

        // Assert
        Assert.IsNotNull(integrationEvent);
    }
}
