using System;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
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
public class CreateTagRequirementPreservationPeriodEventHelperTests
{
    private const string TestPlant = "PCS$PlantA";
    private static DateTime TestTime => DateTime.Parse("2012-12-12T11:22:33Z").ToUniversalTime();
    private static Guid TestGuid => new("11111111-1111-1111-1111-111111111111");
    private const int Interval = 2;

    private TagRequirement _tagRequirement;
    private PreservationPeriod _preservationPeriod;
    private Person _person;
    private CreateTagRequirementPreservationPeriodEventHelper _dut;

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        var timeProvider = new ManualTimeProvider(TestTime);
        TimeService.SetProvider(timeProvider);

        var requirementDefinition = new RequirementDefinition(TestPlant, "D2", 2, RequirementUsage.ForSuppliersOnly, 1);
        _tagRequirement = new TagRequirement(TestPlant, Interval, requirementDefinition);
        _tagRequirement.StartPreservation();

        _preservationPeriod = _tagRequirement.PreservationPeriods.First();

        _person = new Person(TestGuid, "Test", "Person");

        var personRepositoryMock = new Mock<IPersonRepository>();
        personRepositoryMock.Setup(x => x.GetReadOnlyByIdAsync(It.IsAny<int>())).ReturnsAsync(_person);

        _dut = new CreateTagRequirementPreservationPeriodEventHelper(personRepositoryMock.Object);
    }

    [DataTestMethod]
    [DataRow(nameof(PreservationPeriodsEvent.Status), nameof(PreservationPeriodStatus.ReadyToBePreserved))]
    [DataRow(nameof(PreservationPeriodsEvent.Comment), null)]
    [DataRow(nameof(PreservationPeriodsEvent.Plant), TestPlant)]
    [DataRow(nameof(PreservationPeriodsEvent.PreservedAtUtc), null)]
    [DataRow(nameof(PreservationPeriodsEvent.PreservedByGuid), null)]
    [DataRow(nameof(PreservationPeriodsEvent.BulkPreserved), null)]
    public async Task CreateEvent_ShouldCreatePreservationPeriodsEventExpectedValues(string property, object expected)
    {
        // Act
        var integrationEvent = await _dut.CreateEvent(_tagRequirement, _preservationPeriod);
        var result = integrationEvent.GetType()
            .GetProperties()
            .Single(p => p.Name == property)
            .GetValue(integrationEvent);

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public async Task CreateEvent_ShouldCreatePreservationPeriodsEventExpectedProCoSysGuidValue()
    {
        // Arrange
        var expected = _preservationPeriod.Guid;

        // Act
        var integrationEvent = await _dut.CreateEvent(_tagRequirement, _preservationPeriod);
        var result = integrationEvent.ProCoSysGuid;

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public async Task CreateEvent_ShouldCreatePreservationPeriodsEventExpectedCreatedByGuidValue()
    {
        // Act
        var integrationEvent = await _dut.CreateEvent(_tagRequirement, _preservationPeriod);
        var result = integrationEvent.CreatedByGuid;

        // Assert
        Assert.AreEqual(TestGuid, result);
    }

    [TestMethod]
    public async Task CreateEvent_ShouldCreatePreservationPeriodsEventExpectedTagRequirementGuidValue()
    {
        // Arrange
        var expected = _tagRequirement.Guid;

        // Act
        var integrationEvent = await _dut.CreateEvent(_tagRequirement, _preservationPeriod);
        var result = integrationEvent.TagRequirementGuid;

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public async Task CreateEvent_ShouldCreatePreservationPeriodsEventWithExpectedDueTimeUtcValue()
    {
        // Arrange
        var expected = TestTime.AddWeeks(Interval);

        // Act
        var integrationEvent = await _dut.CreateEvent(_tagRequirement, _preservationPeriod);

        // Assert
        Assert.AreEqual(expected, integrationEvent.DueTimeUtc);
    }

    [TestMethod]
    public async Task CreateEvent_ShouldCreatePreservationPeriodsEventWithExpectedCreatedAtUtcValue()
    {
        // Arrange
        _preservationPeriod.SetCreated(_person);

        // Act
        var integrationEvent = await _dut.CreateEvent(_tagRequirement, _preservationPeriod);

        // Assert
        Assert.AreEqual(TestTime, integrationEvent.CreatedAtUtc);
    }

    [TestMethod]
    public async Task CreateEvent_ShouldCreatePreservationPeriodsEventWithModifiedByGuid()
    {
        // Arrange
        _preservationPeriod.SetModified(_person);

        // Act
        var integrationEvent = await _dut.CreateEvent(_tagRequirement, _preservationPeriod);
        var result = integrationEvent.ModifiedByGuid;

        // Assert
        Assert.AreEqual(TestGuid, result);
    }

    [TestMethod]
    public async Task CreateEvent_ShouldCreatePreservationPeriodsEventWithExpectedModifiedAtUtcValue()
    {
        // Arrange
        _preservationPeriod.SetModified(_person);

        // Act
        var integrationEvent = await _dut.CreateEvent(_tagRequirement, _preservationPeriod);

        // Assert
        Assert.AreEqual(TestTime, integrationEvent.ModifiedAtUtc);
    }

    [TestMethod]
    public async Task CreateEvent_ShouldCreatePreservationPeriodsEventWithPreservedByGuid()
    {
        // Arrange
        _preservationPeriod.Preserve(_person, false);

        // Act
        var integrationEvent = await _dut.CreateEvent(_tagRequirement, _preservationPeriod);
        var result = integrationEvent.PreservedByGuid;

        // Assert
        Assert.AreEqual(TestGuid, result);
    }

    [TestMethod]
    public async Task CreateEvent_ShouldCreatePreservationPeriodsEventWithExpectedPreservedAtUtcValue()
    {
        // Arrange
        _preservationPeriod.Preserve(_person, false);

        // Act
        var integrationEvent = await _dut.CreateEvent(_tagRequirement, _preservationPeriod);

        // Assert
        Assert.AreEqual(TestTime, integrationEvent.PreservedAtUtc);
    }

    [TestMethod]
    public async Task CreateEvent_ShouldCreatePreservationPeriodsEventWithExpectedBulkPreservedValue()
    {
        // Arrange
        _preservationPeriod.Preserve(_person, true);

        // Act
        var integrationEvent = await _dut.CreateEvent(_tagRequirement, _preservationPeriod);

        // Assert
        Assert.IsTrue(integrationEvent.BulkPreserved);
    }
}
