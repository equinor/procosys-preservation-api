using System;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.IntegrationEvents;

[TestClass]
public class CreateRequirementDefinitionEventHelperTests
{
    private CreateRequirementDefinitionEventHelper _dut;

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        var mockRequirementTypeRepository = new Mock<IRequirementTypeRepository>();
        mockRequirementTypeRepository.Setup(x => x.GetRequirementTypeByRequirementDefinitionGuidAsync(It.IsAny<Guid>())).ReturnsAsync(new Mock<RequirementType>().Object);

        var createEventHelper = new Mock<ICreateChildEventHelper<RequirementType, RequirementDefinition, RequirementDefinitionEvent>>();
        createEventHelper.Setup(c => c.CreateEvent(It.IsAny<RequirementType>(), It.IsAny<RequirementDefinition>())).ReturnsAsync(new RequirementDefinitionEvent());

        _dut = new CreateRequirementDefinitionEventHelper(mockRequirementTypeRepository.Object, createEventHelper.Object);
    }

    [TestMethod]
    public async Task CreateEvent_ShouldCreateEvent()
    {
        // Arrange
        var requirementDefinition = new RequirementDefinition("PCS$PlantA", "D2", 2, RequirementUsage.ForSuppliersOnly, 1);

        // Act
        var integrationEvent = await _dut.CreateEvent(requirementDefinition);

        // Assert
        Assert.IsNotNull(integrationEvent);
    }
}
