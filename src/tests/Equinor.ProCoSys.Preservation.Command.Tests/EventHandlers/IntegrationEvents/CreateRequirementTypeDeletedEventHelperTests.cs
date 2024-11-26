using System.Linq;
using System.Threading.Tasks;
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
    private CreateRequirementTypeDeletedEventHelper _dut;

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        _requirementType = new RequirementType(TestPlant, "Test Code", "Test Type Title", RequirementTypeIcon.Other, 10);

        _dut = new CreateRequirementTypeDeletedEventHelper();
    }

    [DataTestMethod]
    [DataRow(nameof(RequirementTypeDeleteEvent.Plant), TestPlant)]
    [DataRow(nameof(RequirementTypeDeleteEvent.ProjectName), null)]
    [DataRow(nameof(RequirementTypeDeleteEvent.Behavior), "delete")]
    public async Task CreateEvent_ShouldCreateRequirementDefinitionEventExpectedValues(string property, object expected)
    {
        // Act
        var integrationEvent = await _dut.CreateEvent(_requirementType);
        var result = integrationEvent.GetType()
            .GetProperties()
            .Single(p => p.Name == property)
            .GetValue(integrationEvent);

        // Assert
        Assert.AreEqual(expected, result);
    }
    
    [TestMethod]
    public async Task CreateEvent_ShouldCreateRequirementDefinitionEventWithExpectedProCoSysGuid()
    {
        // Arrange
        var expected = _requirementType.Guid;
        
        // Act
        var integrationEvent = await _dut.CreateEvent(_requirementType);
        var result = integrationEvent.ProCoSysGuid;

        // Assert
        Assert.AreEqual(result, expected);
    }
}
