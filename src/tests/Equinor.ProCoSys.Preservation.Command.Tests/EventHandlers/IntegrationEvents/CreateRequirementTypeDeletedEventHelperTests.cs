using System.Linq;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.IntegrationEvents;

[TestClass]
public class CreateRequirementTypeDeletedEventHelperTests
{
    private const string TestPlant = "PCS$PlantA";
    private RequirementType _requirementType;

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        _requirementType = new RequirementType(TestPlant, "Test Code", "Test Type Title", RequirementTypeIcon.Other, 10);
        
        var requirementDefinition1 = new RequirementDefinition(TestPlant, "Test Definition Title 1", 2, RequirementUsage.ForSuppliersOnly, 1);
        _requirementType.AddRequirementDefinition(requirementDefinition1);
        
        var requirementDefinition2 = new RequirementDefinition(TestPlant, "Test Definition Title 2", 2, RequirementUsage.ForSuppliersOnly, 2);
        _requirementType.AddRequirementDefinition(requirementDefinition2);
    }

    [DataTestMethod]
    [DataRow(nameof(RequirementTypeDeleteEvent.Plant), TestPlant)]
    [DataRow(nameof(RequirementTypeDeleteEvent.ProjectName), null)]
    [DataRow(nameof(RequirementTypeDeleteEvent.Behavior), "delete")]
    public void CreateEvent_ShouldCreateRequirementDefinitionEventExpectedValues(string property, object expected)
    {
        // Act
        var integrationEvents = CreateRequirementTypeDeletedEventHelper.CreateEvents(_requirementType);
        var deletionEvent = integrationEvents.Single(e => e.GetType() == typeof(RequirementTypeDeleteEvent));
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
        var expected = _requirementType.Guid;
        
        // Act
        var integrationEvents = CreateRequirementTypeDeletedEventHelper.CreateEvents(_requirementType);
        var deletionEvent = integrationEvents.Single(e => e.GetType() == typeof(RequirementTypeDeleteEvent));
        var result = deletionEvent.ProCoSysGuid;

        // Assert
        Assert.AreEqual(expected, result);
    }
    
    [TestMethod]
    public void CreateEvent_ShouldCreateIntegrationEventsForChildRequirementDefinitions()
    {
        // Act
        var integrationEvents = CreateRequirementTypeDeletedEventHelper.CreateEvents(_requirementType);
        var requirementDefinitionDeleteEventTypes = integrationEvents.Select(e => e.GetType()).Where(e => e == typeof(RequirementDefinitionDeleteEvent)).ToList();

        // Assert
        Assert.AreEqual(2, requirementDefinitionDeleteEventTypes.Count);
    }
}
