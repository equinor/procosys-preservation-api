using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.ModeCommands.UpdateMode;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.ModeCommands.UpdateMode
{
    [TestClass]
    public class UpdateModeCommandHandlerTests : CommandHandlerTestsBase
    {
        private readonly string _oldTitle = "ModeTitleOld";
        private readonly string _newTitle = "ModeTitleNew";

        private UpdateModeCommand _command;
        private UpdateModeCommandHandler _dut;
        private Mock<Mode> _modeMock;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            var testModeId = 1;
            var modeRepositoryMock = new Mock<IModeRepository>();
            _modeMock = new Mock<Mode>(TestPlant, _oldTitle);
            _modeMock.SetupGet(m => m.Plant).Returns(TestPlant);
            _modeMock.SetupGet(m => m.Id).Returns(testModeId);
            modeRepositoryMock.Setup(m => m.GetByIdAsync(testModeId))
                .Returns(Task.FromResult(_modeMock.Object));
            _command = new UpdateModeCommand(testModeId, _newTitle, null);

            _dut = new UpdateModeCommandHandler(
                modeRepositoryMock.Object,
                UnitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task HandlingUpdateModeCommand_ShouldUpdateMode()
        {
            // Arrange
            Assert.AreEqual(_oldTitle, _modeMock.Object.Title);

            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual("AAAAAAAAAAA=", result.Data);
            Assert.AreEqual(_modeMock.Object.Title, _newTitle);
        }

        [TestMethod]
        public async Task HandlingUpdateModeCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);

            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        [TestMethod]
        public async Task HandlingUpdateModeCommand_ShouldSetRowVersion()
        {
            // Act
            await _dut.Handle(_command, default);

            // Assert
            _modeMock.Verify(u => u.SetRowVersion(_command.RowVersion), Times.Once);
        }
    }
}
