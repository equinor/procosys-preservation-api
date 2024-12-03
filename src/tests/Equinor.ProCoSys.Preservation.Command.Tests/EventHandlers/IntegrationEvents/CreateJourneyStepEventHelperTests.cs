using System;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Time;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.ProCoSys.Preservation.Test.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.IntegrationEvents;

[TestClass]
public class CreateJourneyStepEventHelperTests
{
    private const string TestPlant = "PCS$PlantA";
    private static DateTime TestTime => DateTime.Parse("2012-12-12T11:22:33Z").ToUniversalTime();
    private static Guid TestGuid => new("11111111-1111-1111-1111-111111111111");
    private Mode _mode;
    private Person _person;
    private Responsible _responsible;
    private Step _step;
    private CreateJourneyStepEventHelper _dut;
    private Journey _journey;

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        var timeProvider = new ManualTimeProvider(TestTime);
        TimeService.SetProvider(timeProvider);

        _mode = new Mode(TestPlant, "Test Title", true);

        var mockModeRepository = new Mock<IModeRepository>();
        mockModeRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(_mode);

        _person = new Person(TestGuid, "Test", "Person");

        var mockPersonRepository = new Mock<IPersonRepository>();
        mockPersonRepository.Setup(r => r.GetReadOnlyByIdAsync(It.IsAny<int>())).ReturnsAsync(_person);

        _responsible = new Responsible(TestPlant, "C", "Test Description");
        
        var responsibleRepositoryMock = new Mock<IResponsibleRepository>();
        responsibleRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(_responsible);
        
        _step = new Step(TestPlant, "Test Title", _mode, _responsible);
        
        _dut = new CreateJourneyStepEventHelper(mockModeRepository.Object, mockPersonRepository.Object, responsibleRepositoryMock.Object);
        
        _journey = new Journey(TestPlant, "Test Title");
        _journey.AddStep(_step);
    }

    [DataTestMethod]
    [DataRow(nameof(StepEvent.Plant), TestPlant)]
    [DataRow(nameof(StepEvent.Title), "Test Title")]
    [DataRow(nameof(StepEvent.IsSupplierStep), true)]
    [DataRow(nameof(StepEvent.AutoTransferMethod), "None")]
    [DataRow(nameof(StepEvent.SortKey), 1)]
    [DataRow(nameof(StepEvent.IsVoided), false)]
    public async Task CreateEvent_ShouldCreateStepEventExpectedValues(string property, object expected)
    {
        // Act
        var integrationEvent = await _dut.CreateEvent(_journey, _step);
        var result = integrationEvent.GetType()
            .GetProperties()
            .Single(p => p.Name == property)
            .GetValue(integrationEvent);

        // Assert
        Assert.AreEqual(expected, result);
    }

    [DataTestMethod]
    [DataRow(nameof(StepEvent.CreatedByGuid))]
    [DataRow(nameof(StepEvent.ModifiedByGuid))]
    public async Task CreateEvent_ShouldCreateStepEventWithGuids(string property)
    {
        // Arrange
        _step.SetCreated(_person);
        _step.SetModified(_person);
        
        // Act
        var tagRequirementEvent = await _dut.CreateEvent(_journey, _step);
        var result = tagRequirementEvent.GetType()
            .GetProperties()
            .Single(p => p.Name == property)
            .GetValue(tagRequirementEvent);

        // Assert
        Assert.AreEqual(TestGuid, result);
    }
    
    [TestMethod]
    public async Task CreateEvent_ShouldCreateStepEventWithExpectedProCoSysGuid()
    {
        // Arrange
        var expected = _step.Guid;
        
        // Act
        var integrationEvent = await _dut.CreateEvent(_journey, _step);
        var result = integrationEvent.ProCoSysGuid;

        // Assert
        Assert.AreEqual(expected, result);
    }
    
    [TestMethod]
    public async Task CreateEvent_ShouldCreateStepEventWithExpectedJourneyGuid()
    {
        // Arrange
        var expected = _journey.Guid;
        
        // Act
        var integrationEvent = await _dut.CreateEvent(_journey, _step);
        var result = integrationEvent.JourneyGuid;

        // Assert
        Assert.AreEqual(expected, result);
    }
    
    [TestMethod]
    public async Task CreateEvent_ShouldCreateStepEventWithExpectedModeGuid()
    {
        // Arrange
        var expected = _mode.Guid;
        
        // Act
        var integrationEvent = await _dut.CreateEvent(_journey, _step);
        var result = integrationEvent.ModeGuid;

        // Assert
        Assert.AreEqual(expected, result);
    }
    
    [TestMethod]
    public async Task CreateEvent_ShouldCreateStepEventWithExpectedResponsibleGuid()
    {
        // Arrange
        var expected = _responsible.Guid;
        
        // Act
        var integrationEvent = await _dut.CreateEvent(_journey, _step);
        var result = integrationEvent.ResponsibleGuid;

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public async Task CreateEvent_ShouldCreateStepEventWithExpectedCreatedAtUtcValue()
    {
        // Arrange
        _step.SetCreated(_person);

        // Act
        var tagRequirementEvent = await _dut.CreateEvent(_journey, _step);

        // Assert
        Assert.AreEqual(TestTime, tagRequirementEvent.CreatedAtUtc);
    }

    [TestMethod]
    public async Task CreateEvent_ShouldCreateStepEventWithExpectedModifiedAtUtcValue()
    {
        // Arrange
        _step.SetModified(_person);

        // Act
        var tagRequirementEvent = await _dut.CreateEvent(_journey, _step);

        // Assert
        Assert.AreEqual(TestTime, tagRequirementEvent.ModifiedAtUtc);
    }
}
