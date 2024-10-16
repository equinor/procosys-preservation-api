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
using System.Threading.Tasks;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.IntegrationEvents;

[TestClass]
public class ProjectTagAddedEventConverterTests
{
    protected const string TestPlant = "PCS$PlantA";
    protected readonly DateTime _testTime = DateTime.Parse("2012-12-12T11:22:33Z").ToUniversalTime();
    private Project _project;
    private TagRequirement _tagRequirement;
    private Mock<Step> _stepMock;
    private Tag _tag;
    private ProjectTagAddedEventConverter _dut;
    private Person _person;

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

        _project = new Project(TestPlant, "Test Project", "Test Project Description", Guid.NewGuid());
        _project.AddTag(_tag);

        var mockRequirementTypeRepository = new Mock<IRequirementTypeRepository>();
        mockRequirementTypeRepository.Setup(r => r.GetRequirementDefinitionByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(requirementDefinition));

        _person = new Person(Guid.NewGuid(), "Test", "Person");

        var mockPersonRepository = new Mock<IPersonRepository>();
        mockPersonRepository.Setup(r => r.GetReadOnlyByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(_person));

        _dut = new ProjectTagAddedEventConverter(mockRequirementTypeRepository.Object, mockPersonRepository.Object);
    }

    [DataTestMethod]
    [DataRow("Plant", TestPlant)]
    [DataRow("ProjectName", "Test Project")]
    [DataRow("IntervalWeeks", 2)]
    [DataRow("Usage", nameof(RequirementUsage.ForSuppliersOnly))]
    [DataRow("NextDueTimeUtc", null)]
    [DataRow("IsVoided", false)]
    [DataRow("IsInUse", false)]
    [DataRow("ReadyToBePreserved", false)]
    public async Task Convert_ShouldConvertToTagRequirementWithExpectedValues(string property, object expected)
    {
        // Arrange
        var domainEvent = new ProjectTagAddedEvent(_project, _tag);

        // Act
        var integrationEvents = await _dut.Convert(domainEvent);
        var tagRequirementEvent = integrationEvents.First(e => e.GetType() == typeof(TagRequirementEvent));
        var result = tagRequirementEvent.GetType().GetProperties().Single(p => p.Name == property).GetValue(tagRequirementEvent);

        // Assert
        Assert.AreEqual(expected, result);
    }

    [DataTestMethod]
    [DataRow("ProCoSysGuid")]
    [DataRow("RequirementDefinitionGuid")]
    [DataRow("CreatedByGuid")]
    [DataRow("ModifiedByGuid")]
    public async Task Convert_ShouldConvertToTagRequirementWithGuids(string property)
    {
        // Arrange
        var domainEvent = new ProjectTagAddedEvent(_project, _tag);

        // Act
        var integrationEvents = await _dut.Convert(domainEvent);
        var tagRequirementEvent = integrationEvents.First(e => e.GetType() == typeof(TagRequirementEvent));
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
    public async Task Convert_ShouldConvertToTagRequirementWithExpectedCreatedAtUtcValue()
    {
        // Arrange
        _tagRequirement.SetCreated(_person);

        var domainEvent = new ProjectTagAddedEvent(_project, _tag);

        // Act
        var integrationEvents = await _dut.Convert(domainEvent);
        var tagRequirementEvent = integrationEvents.First(e => e.GetType() == typeof(TagRequirementEvent)) as TagRequirementEvent;

        // Assert
        Assert.AreEqual(_testTime, tagRequirementEvent.CreatedAtUtc);
    }

    [TestMethod]
    public async Task Convert_ShouldConvertToTagRequirementWithExpectedModifiedAtUtcValue()
    {
        // Arrange
        _tagRequirement.SetModified(_person);

        var domainEvent = new ProjectTagAddedEvent(_project, _tag);

        // Act
        var integrationEvents = await _dut.Convert(domainEvent);
        var tagRequirementEvent = integrationEvents.First(e => e.GetType() == typeof(TagRequirementEvent)) as TagRequirementEvent;

        // Assert
        Assert.AreEqual(_testTime, tagRequirementEvent.ModifiedAtUtc);
    }
}
