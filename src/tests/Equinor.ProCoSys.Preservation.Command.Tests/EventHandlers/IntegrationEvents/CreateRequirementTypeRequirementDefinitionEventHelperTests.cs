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
public class CreateRequirementTypeRequirementDefinitionEventHelperTests
{
    private const string TestPlant = "PCS$PlantA";
    private static DateTime TestTime => DateTime.Parse("2012-12-12T11:22:33Z").ToUniversalTime();
    private static Guid TestGuid => new("11111111-1111-1111-1111-111111111111");
    private RequirementType _requirementType;
    private RequirementDefinition _requirementDefinition;
    private Person _person;
    private CreateRequirementTypeRequirementDefinitionEventHelper _dut;

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        var timeProvider = new ManualTimeProvider(TestTime);
        TimeService.SetProvider(timeProvider);

        _requirementType = new RequirementType(TestPlant, "Test Code", "Test Type Title", RequirementTypeIcon.Other, 10);
        _requirementDefinition = new RequirementDefinition(TestPlant, "Test Definition Title", 2, RequirementUsage.ForSuppliersOnly, 1);

        _person = new Person(TestGuid, "Test", "Person");

        var mockPersonRepository = new Mock<IPersonRepository>();
        mockPersonRepository.Setup(r => r.GetReadOnlyByIdAsync(It.IsAny<int>())).ReturnsAsync(_person);

        _dut = new CreateRequirementTypeRequirementDefinitionEventHelper(mockPersonRepository.Object);
    }

    [DataTestMethod]
    [DataRow(nameof(RequirementDefinitionEvent.Plant), TestPlant)]
    [DataRow(nameof(RequirementDefinitionEvent.Title), "Test Definition Title")]
    [DataRow(nameof(RequirementDefinitionEvent.IsVoided), false)]
    [DataRow(nameof(RequirementDefinitionEvent.DefaultIntervalWeeks), 2)]
    [DataRow(nameof(RequirementDefinitionEvent.Usage), "ForSuppliersOnly")]
    [DataRow(nameof(RequirementDefinitionEvent.SortKey), 1)]
    [DataRow(nameof(RequirementDefinitionEvent.NeedsUserInput), false)]
    public async Task CreateEvent_ShouldCreateRequirementDefinitionEventExpectedValues(string property, object expected)
    {
        // Act
        var integrationEvent = await _dut.CreateEvent(_requirementType, _requirementDefinition);
        var result = integrationEvent.GetType()
            .GetProperties()
            .Single(p => p.Name == property)
            .GetValue(integrationEvent);

        // Assert
        Assert.AreEqual(expected, result);
    }

    [DataTestMethod]
    [DataRow(nameof(RequirementDefinitionEvent.CreatedByGuid))]
    [DataRow(nameof(RequirementDefinitionEvent.ModifiedByGuid))]
    public async Task CreateEvent_ShouldCreateRequirementDefinitionEventWithGuids(string property)
    {
        // Arrange
        _requirementDefinition.SetCreated(_person);
        _requirementDefinition.SetModified(_person);

        // Act
        var tagRequirementEvent = await _dut.CreateEvent(_requirementType, _requirementDefinition);
        var result = tagRequirementEvent.GetType()
            .GetProperties()
            .Single(p => p.Name == property)
            .GetValue(tagRequirementEvent);

        // Assert
        Assert.AreEqual(result, TestGuid);
    }

    [TestMethod]
    public async Task CreateEvent_ShouldCreateRequirementDefinitionEventWithExpectedProCoSysGuid()
    {
        // Arrange
        var expected = _requirementDefinition.Guid;

        // Act
        var integrationEvent = await _dut.CreateEvent(_requirementType, _requirementDefinition);
        var result = integrationEvent.ProCoSysGuid;

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public async Task CreateEvent_ShouldCreateRequirementDefinitionEventWithExpectedRequirementTypeGuid()
    {
        // Arrange
        var expected = _requirementType.Guid;

        // Act
        var integrationEvent = await _dut.CreateEvent(_requirementType, _requirementDefinition);
        var result = integrationEvent.RequirementTypeGuid;

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public async Task CreateEvent_ShouldCreateRequirementDefinitionEventWithExpectedCreatedAtUtcValue()
    {
        // Arrange
        _requirementDefinition.SetCreated(_person);

        // Act
        var tagRequirementEvent = await _dut.CreateEvent(_requirementType, _requirementDefinition);

        // Assert
        Assert.AreEqual(TestTime, tagRequirementEvent.CreatedAtUtc);
    }

    [TestMethod]
    public async Task CreateEvent_ShouldCreateRequirementDefinitionEventWithExpectedModifiedAtUtcValue()
    {
        // Arrange
        _requirementDefinition.SetModified(_person);

        // Act
        var tagRequirementEvent = await _dut.CreateEvent(_requirementType, _requirementDefinition);

        // Assert
        Assert.AreEqual(TestTime, tagRequirementEvent.ModifiedAtUtc);
    }
}
