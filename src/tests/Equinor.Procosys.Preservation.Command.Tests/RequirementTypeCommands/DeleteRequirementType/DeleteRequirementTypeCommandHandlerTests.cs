using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.RequirementTypeCommands.DeleteRequirementType;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.RequirementTypeCommands.DeleteRequirementType
{
    [TestClass]
    public class DeleteRequirementTypeCommandHandlerTests : CommandHandlerTestsBase
    {
        private const int RequirementTypeId = 12;
        private const string _rowVersion = "AAAAAAAAABA=";
        private Mock<IRequirementTypeRepository> _requirementTypeRepositoryMock;
        private RequirementType _requirementType;
        private DeleteRequirementTypeCommand _command;
        private DeleteRequirementTypeCommandHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            _requirementTypeRepositoryMock = new Mock<IRequirementTypeRepository>();
            _requirementType = new RequirementType(TestPlant, "Code", "Title", 10);
            _requirementTypeRepositoryMock
                .Setup(x => x.GetByIdAsync(RequirementTypeId))
                    .Returns(Task.FromResult(_requirementType));
            _command = new DeleteRequirementTypeCommand(RequirementTypeId, _rowVersion);

            _dut = new DeleteRequirementTypeCommandHandler(
                _requirementTypeRepositoryMock.Object,
                UnitOfWorkMock.Object
            );
        }

        [TestMethod]
        public async Task HandlingDeleteRequirementTypeCommand_ShouldDeleteRequirementTypeFromRepository()
        {
            // Act
            await _dut.Handle(_command, default);
            
            // Assert
            _requirementTypeRepositoryMock.Verify(r => r.Remove(_requirementType), Times.Once);
        }

        [TestMethod]
        public async Task HandlingDeleteRequirementTypeCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);
            
            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}
