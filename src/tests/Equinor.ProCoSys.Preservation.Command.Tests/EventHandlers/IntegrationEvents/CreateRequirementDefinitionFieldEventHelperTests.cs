using System;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Time;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Test.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.IntegrationEvents;

[TestClass]
public class CreateRequirementDefinitionFieldEventHelperTests
{
    private const string TestPlant = "PCS$PlantA";
    private static DateTime TestTime => DateTime.Parse("2012-12-12T11:22:33Z").ToUniversalTime();
    private static Guid TestGuid => new("11111111-1111-1111-1111-111111111111");
    private RequirementDefinition _requirementDefinition;
    private Field _field;
    private Person _person;
    private CreateRequirementDefinitionFieldEventHelper _dut;

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        var timeProvider = new ManualTimeProvider(TestTime);
        TimeService.SetProvider(timeProvider);
        
        _requirementDefinition = new RequirementDefinition(TestPlant, "Test Definition Title", 2, RequirementUsage.ForSuppliersOnly, 1);
        _field = new Field("PCS$PlantA", "F1", FieldType.Number, 1, "UnitA", true);

        _person = new Person(TestGuid, "Test", "Person");

        var mockPersonRepository = new Mock<IPersonRepository>();
        mockPersonRepository.Setup(r => r.GetReadOnlyByIdAsync(It.IsAny<int>())).ReturnsAsync(_person);

        _dut = new CreateRequirementDefinitionFieldEventHelper(mockPersonRepository.Object);
    }

    [DataTestMethod]
    [DataRow(nameof(FieldEvent.Label), "F1")]
    [DataRow(nameof(FieldEvent.Unit), "UnitA")]
    [DataRow(nameof(FieldEvent.SortKey), 1)]
    [DataRow(nameof(FieldEvent.FieldType), "Number")]
    [DataRow(nameof(FieldEvent.Plant), TestPlant)]
    public async Task CreateEvent_ShouldCreateTagRequirementEventExpectedValues(string property, object expected)
    {
        // Act
        var integrationEvent = await _dut.CreateEvent(_requirementDefinition, _field);
        var result = integrationEvent.GetType()
            .GetProperties()
            .Single(p => p.Name == property)
            .GetValue(integrationEvent);

        // Assert
        Assert.AreEqual(expected, result);
    }

    [DataTestMethod]
    [DataRow(nameof(FieldEvent.CreatedByGuid))]
    [DataRow(nameof(FieldEvent.ModifiedByGuid))]
    public async Task CreateEvent_ShouldCreateTagRequirementEventWithGuids(string property)
    {
        // Arrange
        _field.SetCreated(_person);
        _field.SetModified(_person);
        
        // Act
        var tagRequirementEvent = await _dut.CreateEvent(_requirementDefinition, _field);
        var result = tagRequirementEvent.GetType()
            .GetProperties()
            .Single(p => p.Name == property)
            .GetValue(tagRequirementEvent);

        // Assert
        Assert.AreEqual(result, TestGuid);
    }
    
    [TestMethod]
    public async Task CreateEvent_ShouldCreateTagRequirementEventWithExpectedProCoSysGuid()
    {
        // Arrange
        var expected = _field.Guid;
        
        // Act
        var integrationEvent = await _dut.CreateEvent(_requirementDefinition, _field);
        var result = integrationEvent.ProCoSysGuid;

        // Assert
        Assert.AreEqual(result, expected);
    }
    
    [TestMethod]
    public async Task CreateEvent_ShouldCreateTagRequirementEventWithExpectedRequirementDefinitionGuid()
    {
        // Arrange
        var expected = _requirementDefinition.Guid;
        
        // Act
        var integrationEvent = await _dut.CreateEvent(_requirementDefinition, _field);
        var result = integrationEvent.RequirementDefinitionGuid;

        // Assert
        Assert.AreEqual(result, expected);
    }

    [TestMethod]
    public async Task CreateEvent_ShouldCreateTagRequirementEventWithExpectedCreatedAtUtcValue()
    {
        // Arrange
        _field.SetCreated(_person);

        // Act
        var tagRequirementEvent = await _dut.CreateEvent(_requirementDefinition, _field);

        // Assert
        Assert.AreEqual(TestTime, tagRequirementEvent.CreatedAtUtc);
    }

    [TestMethod]
    public async Task CreateEvent_ShouldCreateTagRequirementEventWithExpectedModifiedAtUtcValue()
    {
        // Arrange
        _field.SetModified(_person);

        // Act
        var tagRequirementEvent = await _dut.CreateEvent(_requirementDefinition, _field);

        // Assert
        Assert.AreEqual(TestTime, tagRequirementEvent.ModifiedAtUtc);
    }
}
