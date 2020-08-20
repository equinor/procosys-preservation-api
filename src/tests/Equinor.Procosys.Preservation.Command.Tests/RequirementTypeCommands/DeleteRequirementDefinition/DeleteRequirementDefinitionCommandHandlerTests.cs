using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.RequirementTypeCommands.DeleteRequirementDefinition;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.RequirementTypeCommands.DeleteRequirementDefinition
{
    [TestClass]
    public class DeleteRequirementDefinitionCommandHandlerTests : CommandHandlerTestsBase
    {
        private int _requirementTypeId = 1;
        private int _requirementDefinitionId = 2;
        private const string _rowVersion = "AAAAAAAAABA=";
        private Mock<IRequirementTypeRepository> _requirementTypeRepositoryMock;
        private DeleteRequirementDefinitionCommand _command;
        private DeleteRequirementDefinitionCommandHandler _dut;
        private Mock<RequirementDefinition> _requirementDefinitionMock;
        private RequirementType _requirementType;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            _requirementTypeRepositoryMock = new Mock<IRequirementTypeRepository>();
            _requirementType = new RequirementType(TestPlant, "Code", "Title", RequirementTypeIcon.Other, 10);
            _requirementDefinitionMock = new Mock<RequirementDefinition>();
            _requirementDefinitionMock.SetupGet(s => s.Plant).Returns(TestPlant);
            _requirementDefinitionMock.SetupGet(s => s.Id).Returns(_requirementDefinitionId);
            _requirementType.AddRequirementDefinition(_requirementDefinitionMock.Object);

            _requirementTypeRepositoryMock
                .Setup(x => x.GetByIdAsync(_requirementTypeId))
                    .Returns(Task.FromResult(_requirementType));
            _command = new DeleteRequirementDefinitionCommand(_requirementTypeId, _requirementDefinitionId, _rowVersion);

            _dut = new DeleteRequirementDefinitionCommandHandler(_requirementTypeRepositoryMock.Object, UnitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task HandlingDeleteRequirementDefinitionCommand_ShouldDeleteRequirementDefinitionFromRequirementType()
        {
            // Arrange
            Assert.AreEqual(1, _requirementType.RequirementDefinitions.Count);

            // Act
            await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, _requirementType.RequirementDefinitions.Count);
            _requirementTypeRepositoryMock.Verify(r => r.RemoveRequirementDefinition(_requirementDefinitionMock.Object), Times.Once);
        }

        [TestMethod]
        public async Task HandlingDeleteRequirementDefinitionCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);
            
            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}
