using System.Collections.Generic;
using System;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.Converters;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Moq;
using Equinor.ProCoSys.Preservation.Domain.Events;
using System.Linq;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Common.Time;
using Equinor.ProCoSys.Preservation.Test.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.IntegrationEvents;

[TestClass]
public class TagCreatedEventConverterTests
{
    protected const string TestPlant = "PCS$PlantA";
    protected readonly DateTime _testTime = DateTime.Parse("2012-12-12T11:22:33Z").ToUniversalTime();
    private TagRequirement _tagRequirement;
    private Mock<Step> _stepMock;
    private Tag _tag;
    private TagCreatedEventConverter _dut;

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        var timeProvider = new ManualTimeProvider(_testTime);
        TimeService.SetProvider(timeProvider);

        var requirementDefinition = new RequirementDefinition(TestPlant, "D2", 2, RequirementUsage.ForSuppliersOnly, 1);
        _tagRequirement = new TagRequirement(TestPlant, 2, requirementDefinition);

        _stepMock = new Mock<Step>();
        _stepMock.SetupGet(s => s.Plant).Returns(TestPlant);
        _tag = new Tag(TestPlant, TagType.Standard, Guid.NewGuid(), "Tag1", "", _stepMock.Object, new List<TagRequirement> { _tagRequirement });

        _dut = new TagCreatedEventConverter();
    }

    [DataTestMethod]
    //[DataRow("ProCoSysGuid", TestPlant)] // TODO
    [DataRow("Plant", TestPlant)]
    //[DataRow("ProjectName", TestPlant)] // TODO
    [DataRow("IntervalWeeks", 2)]
    [DataRow("Usage", nameof(RequirementUsage.ForSuppliersOnly))]
    [DataRow("NextDueTimeUtc", null)]
    [DataRow("IsVoided", false)]
    [DataRow("IsInUse", false)]
    //[DataRow("RequirementDefinitionGuid", TestPlant)] // TODO
    //[DataRow("CreatedByGuid", TestPlant)] // TODO
    //[DataRow("ModifiedAtUtc", TestPlant)] // TODO
    //[DataRow("ModifiedByGuid", TestPlant)] // TODO
    //[DataRow("ReadyToBePreserved", TestPlant)] // TODO
    public void Convert_ShouldConvertToTagRequirementWithExpectedValues(string property, object expected)
    {
        // Arrange
        var domainEvent = new TagCreatedEvent(TestPlant, _tag);

        // Act
        var integrationEvents = _dut.Convert(domainEvent);
        var tagRequirementEvent = integrationEvents.First(e => e.GetType() == typeof(TagRequirementEvent));
        var result = tagRequirementEvent.GetType().GetProperties().Single(p => p.Name == property).GetValue(tagRequirementEvent);

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void Convert_ShouldConvertToTagRequirementWithExpectedCreatedAtUtcValue()
    {
        // Arrange
        var mockPerson = new Mock<Person>();
        _tagRequirement.SetCreated(mockPerson.Object);

        var domainEvent = new TagCreatedEvent(TestPlant, _tag);

        // Act
        var integrationEvents = _dut.Convert(domainEvent);
        var tagRequirementEvent = integrationEvents.First(e => e.GetType() == typeof(TagRequirementEvent)) as TagRequirementEvent;

        // Assert
        Assert.AreEqual(_testTime, tagRequirementEvent.CreatedAtUtc);
    }
}
