using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.Converters;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Equinor.ProCoSys.Preservation.Domain.Events;
using System.Linq;
using Equinor.ProCoSys.Preservation.Command.Events;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.IntegrationEvents;

[TestClass]
public class RequirementTypeRequirementDefinitionAddedEventConverterTests
{
    private const string TestPlant = "PCS$PlantA";
    private RequirementType _requirementType;
    private RequirementDefinition _requirementDefinition;
    private RequirementTypeRequirementDefinitionAddedEventConverter _dut;

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        _requirementType = new RequirementType(TestPlant, "CodeA", "TitleA", RequirementTypeIcon.Other, 10);
        _requirementDefinition = new RequirementDefinition(TestPlant, "D2", 2, RequirementUsage.ForSuppliersOnly, 1);

        var mockTagCreateEvent = new Mock<ICreateChildEventHelper<RequirementType, RequirementDefinition, RequirementDefinitionEvent>>();
        mockTagCreateEvent.Setup(c => c.CreateEvent(It.IsAny<RequirementType>(),It.IsAny<RequirementDefinition>())).ReturnsAsync(new RequirementDefinitionEvent());
        
        _dut = new RequirementTypeRequirementDefinitionAddedEventConverter(mockTagCreateEvent.Object);
    }

    [TestMethod]
    public async Task Convert_ShouldConvertToIntegrationEventsWithRequirementDefinitionEvent()
    {
        // Arrange
        var domainEvent = new RequirementTypeRequirementDefinitionAddedEvent(_requirementType, _requirementDefinition);

        // Act
        var integrationEvents = await _dut.Convert(domainEvent);
        var eventTypes = integrationEvents.Select(e => e.GetType()).ToList();

        // Assert
        CollectionAssert.Contains(eventTypes, typeof(RequirementDefinitionEvent));
    }
}
