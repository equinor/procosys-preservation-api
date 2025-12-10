using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.ModeCommands.DeleteMode;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.ModeCommands.DeleteMode
{
    [TestClass]
    public class DeleteModeCommandHandlerTests : CommandHandlerTestsBase
    {
        private const int ModeId = 12;
        private const string _rowVersion = "AAAAAAAAABA=";
        private Mock<IModeRepository> _modeRepositoryMock;
        private Mode _mode;
        private DeleteModeCommand _command;
        private DeleteModeCommandHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            _modeRepositoryMock = new Mock<IModeRepository>();
            _mode = new Mode(TestPlant, "M", false);
            _modeRepositoryMock
                .Setup(x => x.GetByIdAsync(ModeId))
                    .Returns(Task.FromResult(_mode));
            _command = new DeleteModeCommand(ModeId, _rowVersion);

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
            _modeRepositoryMock.Verify(r => r.Remove(_mode), Times.Once);
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
