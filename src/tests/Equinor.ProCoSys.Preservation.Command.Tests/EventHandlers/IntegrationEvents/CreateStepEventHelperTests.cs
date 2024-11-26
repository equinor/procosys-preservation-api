using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Equinor.ProCoSys.Preservation.Command.Events;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.IntegrationEvents;

[TestClass]
public class CreateStepEventHelperTests
{
    private const string TestPlant = "PCS$PlantA";
    private CreateStepEventHelper _dut;

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        var mockJourneyRepository = new Mock<IJourneyRepository>();
        mockJourneyRepository.Setup(x => x.GetJourneysByStepIdsAsync(It.IsAny<IEnumerable<int>>())).ReturnsAsync([new Mock<Journey>().Object]);
        
        var createEventHelper = new Mock<ICreateChildEventHelper<Journey, Step, StepEvent>>();
        createEventHelper.Setup(c => c.CreateEvent(It.IsAny<Journey>(), It.IsAny<Step>())).ReturnsAsync(new StepEvent());
        
        _dut = new CreateStepEventHelper(mockJourneyRepository.Object, createEventHelper.Object);
    }

    [TestMethod]
    public async Task CreateEvent_ShouldCreateEvent()
    {
        // Arrange
        var mode = new Mode(TestPlant, "Test Title", true);
        var responsible = new Responsible(TestPlant, "C", "Test Description");
        var step = new Step(TestPlant, "Test Title", mode, responsible);
        
        // Act
        var integrationEvent = await _dut.CreateEvent(step);

        // Assert
        Assert.IsNotNull(integrationEvent);
    }
}
