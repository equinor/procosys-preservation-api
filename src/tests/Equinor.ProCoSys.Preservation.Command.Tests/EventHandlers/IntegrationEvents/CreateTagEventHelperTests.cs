using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Time;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Test.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.IntegrationEvents;

[TestClass]
public class CreateTagEventHelperTests
{
    private const string TestPlant = "PCS$PlantA";
    private const string TestProjectName = "Test Project";
    private static DateTime TestTime => DateTime.Parse("2012-12-12T11:22:33Z").ToUniversalTime();
    private Tag _tag;
    private Person _person;
    private CreateTagEventHelper _dut;

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        var timeProvider = new ManualTimeProvider(TestTime);
        TimeService.SetProvider(timeProvider);

        var stepMock = new Mock<Step>();
        stepMock.SetupGet(s => s.Plant).Returns(TestPlant);
        
        var requirementDefinition = new RequirementDefinition(TestPlant, "D2", 2, RequirementUsage.ForSuppliersOnly, 1);
        var tagRequirement = new TagRequirement(TestPlant, 2, requirementDefinition);
        
        _tag = new Tag(TestPlant, TagType.Standard, Guid.NewGuid(), "", "Test Description", stepMock.Object,
            new List<TagRequirement> {tagRequirement})
        {
            Remark = "Test Remark"
        };
        
        _person = new Person(Guid.NewGuid(), "Test", "Person");

        _dut = new CreateTagEventHelper();
    }

    [DataTestMethod]
    [DataRow("Plant", TestPlant)]
    [DataRow("ProjectName", TestProjectName)]
    [DataRow("Description", "Test Description")]
    [DataRow("Remark", "Test Remark")]
    [DataRow("NextDueTimeUtc", "TODO")]
    [DataRow("StepGuid", "TODO")]
    [DataRow("DisciplineCode", "TODO")]
    [DataRow("AreaCode", "TODO")]
    [DataRow("TagFunctionCode", "TODO")]
    [DataRow("PurchaseOrderNo", "TODO")]
    [DataRow("TagType", "TODO")]
    [DataRow("StorageArea", "TODO")]
    [DataRow("AreaDescription", "TODO")]
    [DataRow("DisciplineDescription", "TODO")]
    [DataRow("CreatedAtUtc", "TODO")]
    [DataRow("ModifiedAtUtc", "TODO")]
    [DataRow("Status", "TODO")]
    [DataRow("CommPkgGuid", "TODO")]
    [DataRow("McPkgGuid", "TODO")]
    [DataRow("IsVoided", "TODO")]
    [DataRow("IsVoidedInSource", "TODO")]
    public async Task CreateEvent_ShouldCreateTagEventExpectedValues(string property, object expected)
    {
        // Act
        var integrationEvent = await _dut.CreateEvent(_tag, TestProjectName);
        var result = integrationEvent.GetType()
            .GetProperties()
            .Single(p => p.Name == property)
            .GetValue(integrationEvent);

        // Assert
        Assert.AreEqual(expected, result);
    }

    [DataTestMethod]
    [DataRow("ProCoSysGuid")]
    [DataRow("CreatedByGuid")]
    [DataRow("ModifiedByGuid")]
    public async Task CreateEvent_ShouldCreateTagEventWithGuids(string property)
    {
        // Act
        var tagRequirementEvent = await _dut.CreateEvent(_tag, TestProjectName);
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
    public async Task CreateEvent_ShouldCreateTagEventWithExpectedCreatedAtUtcValue()
    {
        // Arrange
        _tag.SetCreated(_person);

        // Act
        var tagRequirementEvent = await _dut.CreateEvent(_tag, TestProjectName);

        // Assert
        Assert.AreEqual(TestTime, tagRequirementEvent.CreatedAtUtc);
    }

    [TestMethod]
    public async Task CreateEvent_ShouldCreateTagEventWithExpectedModifiedAtUtcValue()
    {
        // Arrange
        _tag.SetModified(_person);

        // Act
        var tagRequirementEvent = await _dut.CreateEvent(_tag, TestProjectName);

        // Assert
        Assert.AreEqual(TestTime, tagRequirementEvent.ModifiedAtUtc);
    }
}
