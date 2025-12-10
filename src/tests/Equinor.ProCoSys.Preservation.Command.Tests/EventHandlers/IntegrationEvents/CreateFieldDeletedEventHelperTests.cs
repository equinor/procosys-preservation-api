using System.Linq;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.IntegrationEvents;

[TestClass]
public class CreateFieldDeletedEventHelperTests
{
    private const string TestPlant = "PCS$PlantA";
    private Field _field;

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        _field = new Field(TestPlant, "Test Label", FieldType.Info, 0);
    }

    [DataTestMethod]
    [DataRow(nameof(FieldDeleteEvent.Plant), TestPlant)]
    [DataRow(nameof(FieldDeleteEvent.ProjectName), null)]
    [DataRow(nameof(FieldDeleteEvent.Behavior), "delete")]
    public void CreateEvent_ShouldCreateFieldDeleteEventExpectedValues(string property, object expected)
    {
        var deletionEvent = CreateFieldDeleteEventHelper.CreateEvent(_field);
        var result = deletionEvent.GetType()
            .GetProperties()
            .Single(p => p.Name == property)
            .GetValue(deletionEvent);

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void CreateEvent_ShouldCreateFieldDeleteEventWithExpectedProCoSysGuid()
    {
        // Arrange
        var expected = _field.Guid;

        // Act
        var deletionEvent = CreateFieldDeleteEventHelper.CreateEvent(_field);
        var result = deletionEvent.ProCoSysGuid;

        // Assert
        Assert.AreEqual(expected, result);
    }
}
