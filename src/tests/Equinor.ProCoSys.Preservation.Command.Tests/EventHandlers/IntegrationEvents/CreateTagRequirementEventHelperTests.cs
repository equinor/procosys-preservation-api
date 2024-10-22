﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Time;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Test.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.IntegrationEvents;

[TestClass]
public class CreateTagRequirementEventHelperTests
{
    private const string TestPlant = "PCS$PlantA";
    private const string TestProjectName = "Test Project";
    private static DateTime TestTime => DateTime.Parse("2012-12-12T11:22:33Z").ToUniversalTime();
    private TagRequirement _tagRequirement;
    private Person _person;
    private CreateTagRequirementEventHelper _dut;

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        var timeProvider = new ManualTimeProvider(TestTime);
        TimeService.SetProvider(timeProvider);

        var requirementDefinition = new RequirementDefinition(TestPlant, "D2", 2, RequirementUsage.ForSuppliersOnly, 1);
        _tagRequirement = new TagRequirement(TestPlant, 2, requirementDefinition);

        var mockRequirementTypeRepository = new Mock<IRequirementTypeRepository>();
        mockRequirementTypeRepository.Setup(r => r.GetRequirementDefinitionByIdAsync(It.IsAny<int>())).ReturnsAsync(requirementDefinition);

        _person = new Person(Guid.NewGuid(), "Test", "Person");

        var mockPersonRepository = new Mock<IPersonRepository>();
        mockPersonRepository.Setup(r => r.GetReadOnlyByIdAsync(It.IsAny<int>())).ReturnsAsync(_person);

        _dut = new CreateTagRequirementEventHelper(mockRequirementTypeRepository.Object, mockPersonRepository.Object);
    }

    [DataTestMethod]
    [DataRow(nameof(TagRequirementEvent.Plant), TestPlant)]
    [DataRow(nameof(TagRequirementEvent.ProjectName), TestProjectName)]
    [DataRow(nameof(TagRequirementEvent.IntervalWeeks), 2)]
    [DataRow(nameof(TagRequirementEvent.Usage), nameof(RequirementUsage.ForSuppliersOnly))]
    [DataRow(nameof(TagRequirementEvent.NextDueTimeUtc), null)]
    [DataRow(nameof(TagRequirementEvent.IsVoided), false)]
    [DataRow(nameof(TagRequirementEvent.IsInUse), false)]
    [DataRow(nameof(TagRequirementEvent.ReadyToBePreserved), false)]
    public async Task CreateEvent_ShouldCreateTagRequirementEventExpectedValues(string property, object expected)
    {
        // Act
        var integrationEvent = await _dut.CreateEvent(_tagRequirement, TestProjectName);
        var result = integrationEvent.GetType()
            .GetProperties()
            .Single(p => p.Name == property)
            .GetValue(integrationEvent);

        // Assert
        Assert.AreEqual(expected, result);
    }

    [DataTestMethod]
    [DataRow(nameof(TagRequirementEvent.ProCoSysGuid))]
    [DataRow(nameof(TagRequirementEvent.RequirementDefinitionGuid))]
    [DataRow(nameof(TagRequirementEvent.CreatedByGuid))]
    [DataRow(nameof(TagRequirementEvent.ModifiedByGuid))]
    public async Task CreateEvent_ShouldCreateTagRequirementEventWithGuids(string property)
    {
        // Arrange
        _tagRequirement.SetCreated(_person);
        _tagRequirement.SetModified(_person);
        
        // Act
        var tagRequirementEvent = await _dut.CreateEvent(_tagRequirement, TestProjectName);
        var result = tagRequirementEvent.GetType()
            .GetProperties()
            .Single(p => p.Name == property)
            .GetValue(tagRequirementEvent);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(result.GetType(), typeof(Guid));
        Assert.AreNotEqual(result, Guid.Empty);
    }

    [TestMethod]
    public async Task CreateEvent_ShouldCreateTagRequirementEventWithExpectedCreatedAtUtcValue()
    {
        // Arrange
        _tagRequirement.SetCreated(_person);

        // Act
        var tagRequirementEvent = await _dut.CreateEvent(_tagRequirement, TestProjectName);

        // Assert
        Assert.AreEqual(TestTime, tagRequirementEvent.CreatedAtUtc);
    }

    [TestMethod]
    public async Task CreateEvent_ShouldCreateTagRequirementEventWithExpectedModifiedAtUtcValue()
    {
        // Arrange
        _tagRequirement.SetModified(_person);

        // Act
        var tagRequirementEvent = await _dut.CreateEvent(_tagRequirement, TestProjectName);

        // Assert
        Assert.AreEqual(TestTime, tagRequirementEvent.ModifiedAtUtc);
    }
}
