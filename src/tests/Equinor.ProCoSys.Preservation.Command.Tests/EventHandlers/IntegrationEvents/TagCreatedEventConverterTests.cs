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

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.IntegrationEvents;

[TestClass]
public class TagCreatedEventConverterTests
{
    protected const string TestPlant = "PCS$PlantA";

    [DataTestMethod]
    //[DataRow("ProCoSysGuid", TestPlant)] // TODO
    [DataRow("Plant", TestPlant)]
    //[DataRow("ProjectName", TestPlant)] // TODO
    [DataRow("IntervalWeeks", 2)]
    [DataRow("Usage", nameof(RequirementUsage.ForSuppliersOnly))]
    [DataRow("NextDueTimeUtc", null)] // TODO
    [DataRow("IsVoided", false)] // TODO
    [DataRow("IsInUse", false)] // TODO
    //[DataRow("RequirementDefinitionGuid", TestPlant)] // TODO
    //[DataRow("CreatedAtUtc", TestPlant)] // TODO
    //[DataRow("CreatedByGuid", TestPlant)] // TODO
    //[DataRow("ModifiedAtUtc", TestPlant)] // TODO
    //[DataRow("ModifiedByGuid", TestPlant)] // TODO
    //[DataRow("ReadyToBePreserved", TestPlant)] // TODO
    public void Convert_ShouldConvertToTagRequirementWithExpectedValues(string property, object expected)
    {
        // Arrange
        var requirementDefinition = new RequirementDefinition(TestPlant, "D2", 2, RequirementUsage.ForSuppliersOnly, 1);
        var tagRequirement = new TagRequirement(TestPlant, 2, requirementDefinition);

        var stepMock = new Mock<Step>();
        stepMock.SetupGet(s => s.Plant).Returns(TestPlant);
        var tag = new Tag(TestPlant, TagType.Standard, Guid.NewGuid(), "Tag1", "", stepMock.Object, new List<TagRequirement> { tagRequirement });

        var domainEvent = new TagCreatedEvent(TestPlant, tag);

        var dut = new TagCreatedEventConverter();

        // Act
        var integrationEvents = dut.Convert(domainEvent);
        var tagRequirementEvent = integrationEvents.First(e => e.GetType() == typeof(TagRequirementEvent));
        var result = tagRequirementEvent.GetType().GetProperties().Single(p => p.Name == property).GetValue(tagRequirementEvent);

        // Assert
        Assert.AreEqual(expected, result);
    }
}
