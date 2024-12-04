using System.Linq;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
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
    }

    [DataTestMethod]
    [DataRow(nameof(RequirementTypeDeleteEvent.Plant), TestPlant)]
    [DataRow(nameof(RequirementTypeDeleteEvent.ProjectName), null)]
    [DataRow(nameof(RequirementTypeDeleteEvent.Behavior), "delete")]
    public void CreateEvent_ShouldCreateRequirementDefinitionEventExpectedValues(string property, object expected)
    {
        var deletionEvent = CreateJourneyDeletedEventHelper.CreateEvent(_journey);
        var result = deletionEvent.GetType()
            .GetProperties()
            .Single(p => p.Name == property)
            .GetValue(deletionEvent);

        // Assert
        Assert.AreEqual(expected, result);
    }
    
    [TestMethod]
    public void CreateEvent_ShouldCreateRequirementDefinitionEventWithExpectedProCoSysGuid()
    {
        // Arrange
        var expected = _journey.Guid;
        
        // Act
        var deletionEvent = CreateJourneyDeletedEventHelper.CreateEvent(_journey);
        var result = deletionEvent.ProCoSysGuid;

        // Assert
        Assert.AreEqual(expected, result);
    }
}
