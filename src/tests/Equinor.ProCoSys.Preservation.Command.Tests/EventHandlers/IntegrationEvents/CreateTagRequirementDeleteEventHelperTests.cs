using System;
using System.Linq;
using Equinor.ProCoSys.Common.Time;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Test.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.IntegrationEvents;

[TestClass]
public class CreateTagRequirementDeleteEventHelperTests
{
    private static DateTime TestTime => DateTime.Parse("2012-12-12T11:22:33Z").ToUniversalTime();
    private const string TestPlant = "PCS$PlantA";
    private TagRequirement _tagRequirement;
    private Project _project;

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        var timeProvider = new ManualTimeProvider(TestTime);
        TimeService.SetProvider(timeProvider);

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
        var deletionEvents = CreateTagRequirementDeleteEventHelper.CreateEvents(_tagRequirement, _project);
        var deletionEvent = deletionEvents.TagRequirementDeleteEvent;
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
        var deletionEvents = CreateTagRequirementDeleteEventHelper.CreateEvents(_tagRequirement, _project);
        var result = deletionEvents.TagRequirementDeleteEvent.ProCoSysGuid;

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void CreateEvents_ShouldCreateIntegrationEventsForChildPreservationPeriods()
    {
        // Arrange
        _tagRequirement.StartPreservation();

        // Act
        var integrationEvents = CreateTagRequirementDeleteEventHelper.CreateEvents(_tagRequirement, _project);
        var result = integrationEvents.PreservationPeriodDeleteEvents;

        // Assert
        Assert.IsTrue(result.Any());
    }
}
