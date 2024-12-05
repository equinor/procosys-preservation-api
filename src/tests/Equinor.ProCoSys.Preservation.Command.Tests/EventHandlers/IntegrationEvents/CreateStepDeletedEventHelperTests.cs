using System.Linq;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.IntegrationEvents;

[TestClass]
public class CreateStepDeletedEventHelperTests
{
    private const string TestPlant = "PCS$PlantA";
    private Step _step;

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        var mode = new Mode(TestPlant, "Test Title", true);
        var responsible = new Responsible(TestPlant, "C", "Test Description");
        _step = new Step(TestPlant, "Test Title", mode, responsible);
    }

    [DataTestMethod]
    [DataRow(nameof(StepDeleteEvent.Plant), TestPlant)]
    [DataRow(nameof(StepDeleteEvent.ProjectName), null)]
    [DataRow(nameof(StepDeleteEvent.Behavior), "delete")]
    public void CreateEvent_ShouldCreateStepDeleteEventExpectedValues(string property, object expected)
    {
        var deletionEvent = CreateStepDeletedEventHelper.CreateEvent(_step);
        var result = deletionEvent.GetType()
            .GetProperties()
            .Single(p => p.Name == property)
            .GetValue(deletionEvent);

        // Assert
        Assert.AreEqual(expected, result);
    }
    
    [TestMethod]
    public void CreateEvent_ShouldCreateStepDeleteEventWithExpectedProCoSysGuid()
    {
        // Arrange
        var expected = _step.Guid;
        
        // Act
        var deletionEvent = CreateStepDeletedEventHelper.CreateEvent(_step);
        var result = deletionEvent.ProCoSysGuid;

        // Assert
        Assert.AreEqual(expected, result);
    }
}
