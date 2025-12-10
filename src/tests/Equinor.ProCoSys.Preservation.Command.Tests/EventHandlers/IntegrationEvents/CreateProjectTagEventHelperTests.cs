using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Common.Time;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.ProCoSys.Preservation.Test.Common;
using Equinor.ProCoSys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.IntegrationEvents;

[TestClass]
public class CreateProjectTagEventHelperTests
{
    private const string TestPlant = "PCS$PlantA";
    private const string TestProjectName = "Test Project";
    private static DateTime TestTime => DateTime.Parse("2012-12-12T11:22:33Z").ToUniversalTime();
    private static Guid TestGuid => new("11111111-1111-1111-1111-111111111111");

    private const int Interval = 2;
    private Step _step;
    private Tag _tag;
    private Project _project;
    private Person _person;
    private CreateProjectTagEventHelper _dut;

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        var timeProvider = new ManualTimeProvider(TestTime);
        TimeService.SetProvider(timeProvider);

        var supplierMode = new Mode(TestPlant, "SUP", true);
        supplierMode.SetProtectedIdForTesting(1);

        var responsible = new Responsible(TestPlant, "C", "D");
        _step = new Step(TestPlant, "SUP", supplierMode, responsible);
        _step.SetProtectedIdForTesting(17);

        var requirementDefinition = new RequirementDefinition(TestPlant, "D2", Interval, RequirementUsage.ForSuppliersOnly, 1);
        var tagRequirement = new TagRequirement(TestPlant, Interval, requirementDefinition);

        _tag = new Tag(TestPlant, TagType.Standard, TestGuid, "", "Test Description", _step,
            new List<TagRequirement> { tagRequirement })
        {
            CommPkgProCoSysGuid = TestGuid,
            McPkgProCoSysGuid = TestGuid,
            PurchaseOrderNo = "Test Purchase Order",
            Remark = "Test Remark",
            StorageArea = "Test Storage Area",
            TagFunctionCode = "Test Function Code",
        };
        _tag.SetProtectedIdForTesting(7);
        _tag.SetArea("A", "A desc");
        _tag.SetDiscipline("D", "D desc");

        _project = new Project(TestPlant, TestProjectName, "Test Project Description", TestGuid);
        _project.AddTag(_tag);

        _person = new Person(TestGuid, "Test", "Person");

        var journeyRepositoryMock = new Mock<IJourneyRepository>();
        journeyRepositoryMock.Setup(x => x.GetStepByStepIdAsync(It.IsAny<int>())).ReturnsAsync(_step);

        var personRepositoryMock = new Mock<IPersonRepository>();
        personRepositoryMock.Setup(x => x.GetReadOnlyByIdAsync(It.IsAny<int>())).ReturnsAsync(_person);

        _dut = new CreateProjectTagEventHelper(journeyRepositoryMock.Object, personRepositoryMock.Object);
    }

    [DataTestMethod]
    [DataRow(nameof(TagEvent.Plant), TestPlant)]
    [DataRow(nameof(TagEvent.ProjectName), TestProjectName)]
    [DataRow(nameof(TagEvent.Remark), "Test Remark")]
    [DataRow(nameof(TagEvent.TagType), "Standard")]
    [DataRow(nameof(TagEvent.StorageArea), "Test Storage Area")]
    [DataRow(nameof(TagEvent.Status), "NotStarted")]
    [DataRow(nameof(TagEvent.IsVoided), false)]
    public async Task CreateEvent_ShouldCreateTagEventExpectedValues(string property, object expected)
    {
        // Act
        var integrationEvent = await _dut.CreateEvent(_project, _tag);
        var result = integrationEvent.GetType()
            .GetProperties()
            .Single(p => p.Name == property)
            .GetValue(integrationEvent);

        // Assert
        Assert.AreEqual(expected, result);
    }

    [DataTestMethod]
    [DataRow(nameof(TagEvent.CommPkgGuid))]
    [DataRow(nameof(TagEvent.CreatedByGuid))]
    [DataRow(nameof(TagEvent.McPkgGuid))]
    [DataRow(nameof(TagEvent.ProCoSysGuid))]
    public async Task CreateEvent_ShouldCreateTagEventWithGuids(string property)
    {
        // Act
        var integrationEvent = await _dut.CreateEvent(_project, _tag);
        var result = integrationEvent.GetType()
            .GetProperties()
            .Single(p => p.Name == property)
            .GetValue(integrationEvent);

        // Assert
        Assert.AreEqual(result, TestGuid);
    }

    [TestMethod]
    public async Task CreateEvent_ShouldCreateTagEventWithExpectedStepGuid()
    {
        // Arrange
        var expected = _step.Guid;

        // Act
        var integrationEvent = await _dut.CreateEvent(_project, _tag);
        var result = integrationEvent.StepGuid;

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public async Task CreateEvent_ShouldCreateTagEventWithExpectedCreatedAtUtcValue()
    {
        // Arrange
        _tag.SetCreated(_person);

        // Act
        var integrationEvent = await _dut.CreateEvent(_project, _tag);

        // Assert
        Assert.AreEqual(TestTime, integrationEvent.CreatedAtUtc);
    }

    [TestMethod]
    public async Task CreateEvent_ShouldCreateTagEventWithModifiedByGuid()
    {
        // Arrange
        _tag.SetModified(_person);

        // Act
        var integrationEvent = await _dut.CreateEvent(_project, _tag);
        var result = integrationEvent.ModifiedByGuid;

        // Assert
        Assert.AreEqual(result, TestGuid);
    }

    [TestMethod]
    public async Task CreateEvent_ShouldCreateTagEventWithExpectedModifiedAtUtcValue()
    {
        // Arrange
        _tag.SetModified(_person);

        // Act
        var integrationEvent = await _dut.CreateEvent(_project, _tag);

        // Assert
        Assert.AreEqual(TestTime, integrationEvent.ModifiedAtUtc);
    }

    [TestMethod]
    public async Task CreateEvent_ShouldCreateTagEventWithExpectedNextDueTimeUtcValue()
    {
        // Arrange
        _tag.StartPreservation();
        var expected = TestTime.AddWeeks(Interval);

        // Act
        var integrationEvent = await _dut.CreateEvent(_project, _tag);

        // Assert
        Assert.AreEqual(expected, integrationEvent.NextDueTimeUtc);
    }
}
