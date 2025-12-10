using System;
using System.Linq;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.IntegrationEvents;

[TestClass]
public class CreateActionDeletedEventHelperTests
{
    private const string TestPlant = "PCS$PlantA";
    private Action _action;
    private Project _project;

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        _action = new Action(TestPlant, "Test Action", "Test Action Description", null);
        _project = new Project(TestPlant, "Test Project", "Test Project Description", Guid.NewGuid());
    }

    [DataTestMethod]
    [DataRow(nameof(ActionDeleteEvent.Plant), TestPlant)]
    [DataRow(nameof(ActionDeleteEvent.ProjectName), "Test Project")]
    [DataRow(nameof(ActionDeleteEvent.Behavior), "delete")]
    public void CreateEvent_ShouldCreateActionDeleteEventExpectedValues(string property, object expected)
    {
        var deletionEvent = CreateActionDeletedEventHelper.CreateEvent(_action, _project);
        var result = deletionEvent.GetType()
            .GetProperties()
            .Single(p => p.Name == property)
            .GetValue(deletionEvent);

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void CreateEvent_ShouldCreateActionDeleteEventWithExpectedProCoSysGuid()
    {
        // Arrange
        var expected = _action.Guid;

        // Act
        var deletionEvent = CreateActionDeletedEventHelper.CreateEvent(_action, _project);
        var result = deletionEvent.ProCoSysGuid;

        // Assert
        Assert.AreEqual(expected, result);
    }
}
