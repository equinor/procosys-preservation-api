using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.ModeCommands.DeleteMode;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.ModeCommands.DeleteMode
{
    [TestClass]
    public class DeleteModeCommandHandlerTests : CommandHandlerTestsBase
    {
        private const int ModeId = 12;
        private Mock<IModeRepository> _modeRepositoryMock;
        private Mock<Mode> _modeMock;
        private DeleteModeCommand _command;
        private DeleteModeCommandHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            _modeRepositoryMock = new Mock<IModeRepository>();
            _modeMock = new Mock<Mode>();
            _modeMock.SetupGet(m => m.Id).Returns(ModeId);
            _modeRepositoryMock
                .Setup(x => x.GetByIdAsync(ModeId))
                    .Returns(Task.FromResult(_modeMock.Object));

            _command = new DeleteModeCommand(TestPlant, ModeId);

            _dut = new DeleteModeCommandHandler(
                _modeRepositoryMock.Object,
                UnitOfWorkMock.Object
            );
        }

        [TestMethod]
        public async Task HandlingDeleteModeCommand_ShouldDeleteModeFromRepository()
        {
            // Act
            await _dut.Handle(_command, default);
            
            // Assert
            _modeRepositoryMock.Verify(r => r.Remove(_modeMock.Object), Times.Once);
        }

        [TestMethod]
        public async Task HandlingDeleteModeCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);
            
            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}
