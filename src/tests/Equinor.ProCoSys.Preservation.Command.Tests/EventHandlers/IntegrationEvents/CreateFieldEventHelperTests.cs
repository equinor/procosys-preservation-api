using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Equinor.ProCoSys.Preservation.Command.Events;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.IntegrationEvents;

[TestClass]
public class CreateFieldEventHelperTests
{
    private CreateFieldEventHelper _dut;

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        var mockRequirementTypeRepository = new Mock<IRequirementTypeRepository>();
        mockRequirementTypeRepository.Setup(x => x.GetRequirementTypeByRequirementDefinitionGuidAsync(It.IsAny<Guid>())).ReturnsAsync(new Mock<RequirementType>().Object);
        
        var createEventHelper = new Mock<ICreateChildEventHelper<RequirementDefinition, Field, FieldEvent>>();
        createEventHelper.Setup(c => c.CreateEvent(It.IsAny<RequirementDefinition>(), It.IsAny<Field>())).ReturnsAsync(new FieldEvent());
        
        _dut = new CreateFieldEventHelper(mockRequirementTypeRepository.Object, createEventHelper.Object);
    }

    [TestMethod]
    public async Task CreateEvent_ShouldCreateEvent()
    {
        // Arrange
        var action = new Field("PCS$PlantA", "F1", FieldType.Number, 1, "UnitA", true);
        
        // Act
        var integrationEvent = await _dut.CreateEvent(action);

        // Assert
        Assert.IsNotNull(integrationEvent);
    }
}
