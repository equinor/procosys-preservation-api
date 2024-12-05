using System.Linq;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.IntegrationEvents;

[TestClass]
public class CreateJourneyDeletedEventHelperTests
{
    private const string TestPlant = "PCS$PlantA";
    private Journey _journey;

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        _journey = new Journey(TestPlant, "Test Title");
        
        var mode = new Mode(TestPlant, "Test Title", true);
        var responsible = new Responsible(TestPlant, "C", "Test Description");
        
        var step1 = new Step(TestPlant, "Test Title 1", mode, responsible);
        _journey.AddStep(step1);
        
        var step2 = new Step(TestPlant, "Test Title 2", mode, responsible);
        _journey.AddStep(step2);
    }

    [DataTestMethod]
    [DataRow(nameof(JourneyDeleteEvent.Plant), TestPlant)]
    [DataRow(nameof(JourneyDeleteEvent.ProjectName), null)]
    [DataRow(nameof(JourneyDeleteEvent.Behavior), "delete")]
    public void CreateEvent_ShouldCreateIntegrationEventExpectedValues(string property, object expected)
    {
        var deletionEvents = CreateJourneyDeletedEventHelper.CreateEvent(_journey);
        var deletionEvent = deletionEvents.JourneyDeleteEvent;
        var result = deletionEvent.GetType()
            .GetProperties()
            .Single(p => p.Name == property)
            .GetValue(deletionEvent);

        // Assert
        Assert.AreEqual(expected, result);
    }
    
    [TestMethod]
    public void CreateEvent_ShouldCreateJourneyDeletedEventWithExpectedProCoSysGuid()
    {
        // Arrange
        var expected = _journey.Guid;
        
        // Act
        var deletedEvents = CreateJourneyDeletedEventHelper.CreateEvent(_journey);
        var result = deletedEvents.JourneyDeleteEvent.ProCoSysGuid;

        // Assert
        Assert.AreEqual(expected, result);
    }
    
    [TestMethod]
    public void CreateEvent_ShouldCreateIntegrationEventsForChildSteps()
    {
        // Act
        var integrationEvents = CreateJourneyDeletedEventHelper.CreateEvent(_journey);

        // Assert
        Assert.AreEqual(2, integrationEvents.StepDeleteEvents.Count());
    }
}
