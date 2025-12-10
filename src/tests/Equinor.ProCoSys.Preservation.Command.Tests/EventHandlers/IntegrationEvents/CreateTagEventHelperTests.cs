using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.IntegrationEvents;

[TestClass]
public class CreateTagEventHelperTests
{
    private const string TestPlant = "PlantA";
    private CreateTagEventHelper _dut;

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        var mockProjectRepository = new Mock<IProjectRepository>();
        mockProjectRepository.Setup(x => x.GetTagByActionGuidAsync(It.IsAny<Guid>())).ReturnsAsync(new Mock<Tag>().Object);

        var createEventHelper = new Mock<ICreateChildEventHelper<Project, Tag, TagEvent>>();
        createEventHelper.Setup(c => c.CreateEvent(It.IsAny<Project>(), It.IsAny<Tag>())).ReturnsAsync(new TagEvent());

        _dut = new CreateTagEventHelper(mockProjectRepository.Object, createEventHelper.Object);
    }

    [TestMethod]
    public async Task CreateEvent_ShouldCreateEvent()
    {
        // Arrange
        var stepMock = new Mock<Step>();
        stepMock.SetupGet(s => s.Plant).Returns(TestPlant);

        var tagRequirementMock = new Mock<TagRequirement>();
        tagRequirementMock.SetupGet(r => r.Plant).Returns(TestPlant);
        var tag = new Tag(TestPlant, TagType.Standard, Guid.NewGuid(), "Tag1", "", stepMock.Object,
            new List<TagRequirement> { tagRequirementMock.Object });

        // Act
        var integrationEvent = await _dut.CreateEvent(tag);

        // Assert
        Assert.IsNotNull(integrationEvent);
    }
}
