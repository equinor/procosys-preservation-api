using System.Linq;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.IntegrationEvents;

[TestClass]
public class CreateRequirementDefinitionDeletedEventHelperTests
{
    private const string TestPlant = "PCS$PlantA";
    private RequirementDefinition _requirementDefinition;

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        _requirementDefinition = new RequirementDefinition(TestPlant, "Test Definition", 2, RequirementUsage.ForAll, 1);
        
        var field1 = new Field(TestPlant, "Test Label 1", FieldType.Info, 1);
        _requirementDefinition.AddField(field1);
        
        var field2 = new Field(TestPlant, "Test Label 2", FieldType.Info, 2);
        _requirementDefinition.AddField(field2);
    }

    [DataTestMethod]
    [DataRow(nameof(RequirementTypeDeleteEvent.Plant), TestPlant)]
    [DataRow(nameof(RequirementTypeDeleteEvent.ProjectName), null)]
    [DataRow(nameof(RequirementTypeDeleteEvent.Behavior), "delete")]
    public void CreateEvent_ShouldCreateRequirementDefinitionEventExpectedValues(string property, object expected)
    {
        var integrationEvents = CreateRequirementDefinitionDeletedEventHelper.CreateEvents(_requirementDefinition);
        var deletionEvent = integrationEvents.Single(e => e.GetType() == typeof(RequirementDefinitionDeleteEvent));
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
        var expected = _requirementDefinition.Guid;
        
        // Act
        var integrationEvents = CreateRequirementDefinitionDeletedEventHelper.CreateEvents(_requirementDefinition);
        var deletionEvent = integrationEvents.Single(e => e.GetType() == typeof(RequirementDefinitionDeleteEvent));
        var result = deletionEvent.ProCoSysGuid;

        // Assert
        Assert.AreEqual(result, expected);
    }
    
    [TestMethod]
    public void CreateEvent_ShouldCreateIntegrationEventsForChildRequirementDefinitions()
    {
        // Act
        var integrationEvents = CreateRequirementDefinitionDeletedEventHelper.CreateEvents(_requirementDefinition);
        var requirementDefinitionDeleteEventTypes = integrationEvents.Select(e => e.GetType()).Where(e => e == typeof(FieldDeleteEvent)).ToList();

        // Assert
        Assert.AreEqual(2, requirementDefinitionDeleteEventTypes.Count);
    }
}
