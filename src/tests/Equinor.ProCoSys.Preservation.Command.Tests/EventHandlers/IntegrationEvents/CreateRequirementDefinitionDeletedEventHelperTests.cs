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
    [DataRow(nameof(RequirementDefinitionDeleteEvent.Plant), TestPlant)]
    [DataRow(nameof(RequirementDefinitionDeleteEvent.ProjectName), null)]
    [DataRow(nameof(RequirementDefinitionDeleteEvent.Behavior), "delete")]
    public void CreateEvent_ShouldCreateRequirementDefinitionDeleteEventExpectedValues(string property, object expected)
    {
        var integrationEvents = CreateRequirementDefinitionDeletedEventHelper.CreateEvents(_requirementDefinition);
        var deletionEvent = integrationEvents.DefinitionDeleteEvent;
        var result = deletionEvent.GetType()
            .GetProperties()
            .Single(p => p.Name == property)
            .GetValue(deletionEvent);

        // Assert
        Assert.AreEqual(expected, result);
    }
    
    [TestMethod]
    public void CreateEvent_ShouldCreateRequirementDefinitionDeleteEventWithExpectedProCoSysGuid()
    {
        // Arrange
        var expected = _requirementDefinition.Guid;
        
        // Act
        var integrationEvents = CreateRequirementDefinitionDeletedEventHelper.CreateEvents(_requirementDefinition);
        var result = integrationEvents.DefinitionDeleteEvent.ProCoSysGuid;

        // Assert
        Assert.AreEqual(expected, result);
    }
    
    [TestMethod]
    public void CreateEvent_ShouldCreateIntegrationEventsForChildRequirementDefinitions()
    {
        // Act
        var integrationEvents = CreateRequirementDefinitionDeletedEventHelper.CreateEvents(_requirementDefinition);

        // Assert
        Assert.AreEqual(2, integrationEvents.FieldDeleteEvents.Count());
    }
}
