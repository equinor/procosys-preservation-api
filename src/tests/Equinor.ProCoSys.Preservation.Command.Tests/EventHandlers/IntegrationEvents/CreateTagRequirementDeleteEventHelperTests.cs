using System;
using System.Linq;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.IntegrationEvents;

[TestClass]
public class CreateTagRequirementDeleteEventHelperTests
{
    private const string TestPlant = "PCS$PlantA";
    private TagRequirement _tagRequirement;
    private Project _project;

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        var requirementDefinition = new RequirementDefinition(TestPlant, "Requirement Definition", 1, RequirementUsage.ForSuppliersOnly, 1);
        _tagRequirement = new TagRequirement(TestPlant, 1, requirementDefinition);
        _project = new Project(TestPlant, "Test Project", "Test Project Description", Guid.NewGuid());
    }

    [DataTestMethod]
    [DataRow(nameof(ActionDeleteEvent.Plant), TestPlant)]
    [DataRow(nameof(ActionDeleteEvent.ProjectName), "Test Project")]
    [DataRow(nameof(ActionDeleteEvent.Behavior), "delete")]
    public void CreateEvent_ShouldCreateActionDeleteEventExpectedValues(string property, object expected)
    {
        var deletionEvent = CreateTagRequirementDeleteEventHelper.CreateEvent(_tagRequirement, _project);
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
        var expected = _tagRequirement.Guid;
        
        // Act
        var deletionEvent = CreateTagRequirementDeleteEventHelper.CreateEvent(_tagRequirement, _project);
        var result = deletionEvent.ProCoSysGuid;

        // Assert
        Assert.AreEqual(expected, result);
    }
}
