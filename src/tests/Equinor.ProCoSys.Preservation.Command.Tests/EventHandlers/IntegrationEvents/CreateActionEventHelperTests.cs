using System;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.IntegrationEvents;

[TestClass]
public class CreateActionEventHelperTests
{
    private CreateActionEventHelper _dut;

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        var mockProjectRepository = new Mock<IProjectRepository>();
        mockProjectRepository.Setup(x => x.GetTagByActionGuidAsync(It.IsAny<Guid>())).ReturnsAsync(new Mock<Tag>().Object);

        var createEventHelper = new Mock<ICreateChildEventHelper<Tag, Action, ActionEvent>>();
        createEventHelper.Setup(c => c.CreateEvent(It.IsAny<Tag>(), It.IsAny<Action>())).ReturnsAsync(new ActionEvent());

        _dut = new CreateActionEventHelper(mockProjectRepository.Object, createEventHelper.Object);
    }

    [TestMethod]
    public async Task CreateEvent_ShouldCreateEvent()
    {
        // Arrange
        var action = new Action("PCS$PlantA", "Test Action", "Test Action Description", null);

        // Act
        var integrationEvent = await _dut.CreateEvent(action);

        // Assert
        Assert.IsNotNull(integrationEvent);
    }
}
