using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.IntegrationEvents;

[TestClass]
public class CreateTagActionEventHelperTests
{
    private const string TestPlant = "PCS$PlantA";
    private const string TestProjectName = "Test Project";
    private static DateTime TestTime => DateTime.Parse("2012-12-12T11:22:33Z").ToUniversalTime();
    private static Guid TestGuid => new("11111111-1111-1111-1111-111111111111");

    private Tag _tag;
    private Person _person;
    private Action _action;
    private CreateTagActionEventHelper _dut;

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        var timeProvider = new ManualTimeProvider(TestTime);
        TimeService.SetProvider(timeProvider);
        
        var supplierMode = new Mode(TestPlant, "SUP", true);
        var responsible = new Responsible(TestPlant, "C", "D");
        var step = new Step(TestPlant, "SUP", supplierMode, responsible);

        var requirementDefinition = new RequirementDefinition(TestPlant, "D2", 2, RequirementUsage.ForSuppliersOnly, 1);
        var tagRequirement = new TagRequirement(TestPlant, 2, requirementDefinition);
        
        _tag = new Tag(TestPlant, TagType.Standard, TestGuid, "", "Test Description", step, new List<TagRequirement> {tagRequirement})
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
        
        _person = new Person(TestGuid, "Test", "Person");
        
        _action = new Action(TestPlant, "Test Action", "Test Action Description", TestTime);
        _tag.AddAction(_action);

        var personRepositoryMock = new Mock<IPersonRepository>();
        personRepositoryMock.Setup(x => x.GetReadOnlyByIdAsync(It.IsAny<int>())).ReturnsAsync(_person);
        
        var project = new Project(TestPlant, TestProjectName, "Test Project Description", TestGuid);
        project.AddTag(_tag);
        
        var projectRepositoryMock = new Mock<IProjectRepository>();
        projectRepositoryMock.Setup(x => x.GetProjectOnlyByTagGuidAsync(It.IsAny<Guid>())).ReturnsAsync(project);
        
        _dut = new CreateTagActionEventHelper(personRepositoryMock.Object, projectRepositoryMock.Object);
    }

    [DataTestMethod]
    [DataRow(nameof(ActionEvent.Plant), TestPlant)]
    [DataRow(nameof(ActionEvent.ProjectName), TestProjectName)]
    [DataRow(nameof(ActionEvent.Title), "Test Action")]
    [DataRow(nameof(ActionEvent.Description), "Test Action Description")]
    [DataRow(nameof(ActionEvent.Overdue), false)]
    public async Task CreateEvent_ShouldCreateActionEventExpectedValues(string property, object expected)
    {
        // Act
        var integrationEvent = await _dut.CreateEvent(_tag, _action);
        var result = integrationEvent.GetType()
            .GetProperties()
            .Single(p => p.Name == property)
            .GetValue(integrationEvent);

        // Assert
        Assert.AreEqual(result, expected);
    }

    [DataTestMethod]
    [DataRow(nameof(ActionEvent.CreatedByGuid))]
    [DataRow(nameof(ActionEvent.TagGuid))]
    public async Task CreateEvent_ShouldCreateActionEventWithGuids(string property)
    {
        // Act
        var integrationEvent = await _dut.CreateEvent(_tag, _action);
        var result = integrationEvent.GetType()
            .GetProperties()
            .Single(p => p.Name == property)
            .GetValue(integrationEvent);

        // Assert
        Assert.AreEqual(TestGuid, result);
    }
    
    [TestMethod]
    public async Task CreateEvent_ShouldCreateActionEventExpectedProCoSysGuidValue()
    {
        // Arrange
        var expected = _action.Guid;
        
        // Act
        var integrationEvent = await _dut.CreateEvent(_tag, _action);
        var result = integrationEvent.ProCoSysGuid;

        // Assert
        Assert.AreEqual(expected, result);
    }
    
    [TestMethod]
    public async Task CreateEvent_ShouldCreateActionEventExpectedDueDateValue()
    {
        // Arrange
        var expected = DateOnly.FromDateTime(TestTime);  
        
        // Act
        var integrationEvent = await _dut.CreateEvent(_tag, _action);
        var result = integrationEvent.DueDate;

        // Assert
        Assert.AreEqual(expected, result);
    }
    
    [TestMethod]
    public async Task CreateEvent_ShouldCreateActionEventExpectedClosedValue()
    {
        // Arrange
        var expected = DateOnly.FromDateTime(TestTime);
        _action.Close(TestTime, _person);
        
        // Act
        var integrationEvent = await _dut.CreateEvent(_tag, _action);
        var result = integrationEvent.Closed;

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public async Task CreateEvent_ShouldCreateActionEventWithExpectedCreatedAtUtcValue()
    {
        // Arrange
        _action.SetCreated(_person);

        // Act
        var integrationEvent = await _dut.CreateEvent(_tag, _action);

        // Assert
        Assert.AreEqual(TestTime, integrationEvent.CreatedAtUtc);
    }
    
    [TestMethod]
    public async Task CreateEvent_ShouldCreateActionEventWithModifiedByGuid()
    {
        // Arrange
        _action.SetModified(_person);
        
        // Act
        var integrationEvent = await _dut.CreateEvent(_tag, _action);
        var result = integrationEvent.ModifiedByGuid;

        // Assert
        Assert.AreEqual(TestGuid, result);
    }

    [TestMethod]
    public async Task CreateEvent_ShouldCreateActionEventWithExpectedModifiedAtUtcValue()
    {
        // Arrange
        _action.SetModified(_person);

        // Act
        var integrationEvent = await _dut.CreateEvent(_tag, _action);

        // Assert
        Assert.AreEqual(TestTime, integrationEvent.ModifiedAtUtc);
    }
}
