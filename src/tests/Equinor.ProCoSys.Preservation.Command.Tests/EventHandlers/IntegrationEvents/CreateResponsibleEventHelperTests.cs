using System;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Time;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.ProCoSys.Preservation.Test.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.IntegrationEvents;

[TestClass]
public class CreateResponsibleEventHelperTests
{
    private const string TestPlant = "PCS$PlantA";
    private static DateTime TestTime => DateTime.Parse("2012-12-12T11:22:33Z").ToUniversalTime();
    private static Guid TestGuid => new("11111111-1111-1111-1111-111111111111");

    private Responsible _responsible;
    private Person _person;
    private CreateResponsibleEventHelper _dut;

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        var timeProvider = new ManualTimeProvider(TestTime);
        TimeService.SetProvider(timeProvider);
        
        _responsible = new Responsible(TestPlant, "Test Code", "Test description");
        
        _person = new Person(TestGuid, "Test", "Person");

        var personRepositoryMock = new Mock<IPersonRepository>();
        personRepositoryMock.Setup(x => x.GetReadOnlyByIdAsync(It.IsAny<int>())).ReturnsAsync(_person);
        
        _dut = new CreateResponsibleEventHelper(personRepositoryMock.Object);
    }

    [DataTestMethod]
    [DataRow(nameof(ResponsibleEvent.Plant), TestPlant)]
    [DataRow(nameof(ResponsibleEvent.IsVoided), false)]
    public async Task CreateEvent_ShouldCreateResponsibleEventWithExpectedValues(string property, object expected)
    {
        // Act
        var integrationEvent = await _dut.CreateEvent(_responsible);
        var result = integrationEvent.GetType()
            .GetProperties()
            .Single(p => p.Name == property)
            .GetValue(integrationEvent);

        // Assert
        Assert.AreEqual(result, expected);
    }
    
    [TestMethod]
    public async Task CreateEvent_ShouldCreateActionEventExpectedProCoSysGuidValue()
    {
        // Arrange
        var expected = _responsible.Guid;
        
        // Act
        var integrationEvent = await _dut.CreateEvent(_responsible);
        var result = integrationEvent.ProCoSysGuid;

        // Assert
        Assert.AreEqual(expected, result);
    }
    
    [TestMethod]
    public async Task CreateEvent_ShouldCreateActionEventExpectedCreatedByGuidValue()
    {
        // Arrange
        _responsible.SetCreated(_person);
        
        // Act
        var integrationEvent = await _dut.CreateEvent(_responsible);
        var result = integrationEvent.CreatedByGuid;

        // Assert
        Assert.AreEqual(TestGuid, result);
    }

    [TestMethod]
    public async Task CreateEvent_ShouldCreateActionEventWithExpectedCreatedAtUtcValue()
    {
        // Arrange
        _responsible.SetCreated(_person);

        // Act
        var integrationEvent = await _dut.CreateEvent(_responsible);

        // Assert
        Assert.AreEqual(TestTime, integrationEvent.CreatedAtUtc);
    }
    
    [TestMethod]
    public async Task CreateEvent_ShouldCreateActionEventWithModifiedByGuid()
    {
        // Arrange
        _responsible.SetModified(_person);
        
        // Act
        var integrationEvent = await _dut.CreateEvent(_responsible);
        var result = integrationEvent.ModifiedByGuid;

        // Assert
        Assert.AreEqual(TestGuid, result);
    }

    [TestMethod]
    public async Task CreateEvent_ShouldCreateActionEventWithExpectedModifiedAtUtcValue()
    {
        // Arrange
        _responsible.SetModified(_person);

        // Act
        var integrationEvent = await _dut.CreateEvent(_responsible);

        // Assert
        Assert.AreEqual(TestTime, integrationEvent.ModifiedAtUtc);
    }
}
