using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Command.ModeCommands.VoidMode;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.ModeCommands.VoidMode
{
    [TestClass]
    public class VoidModeCommandHandlerTests : CommandHandlerTestsBase
    {
        private Mode _mode;
        private VoidModeCommand _command;
        private VoidModeCommandHandler _dut;
        private readonly string _rowVersion = "AAAAAAAAABA=";

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            var modeId = 1;
            var modeRepositoryMock = new Mock<IModeRepository>();

            _mode = new Mode(TestPlant, "ModeTitle", false);
            _mode.SetProtectedIdForTesting(modeId);

            modeRepositoryMock.Setup(m => m.GetByIdAsync(modeId))
                .Returns(Task.FromResult(_mode));

            _command = new VoidModeCommand(modeId, _rowVersion);

            _dut = new VoidModeCommandHandler(
                modeRepositoryMock.Object,
                UnitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task HandlingVoidModeCommand_ShouldVoidMode()
        {
            // Arrange
            Assert.IsFalse(_mode.IsVoided);

            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.IsTrue(_mode.IsVoided);
        }

        [TestMethod]
        public async Task HandlingVoidModeCommand_ShouldSetAndReturnRowVersion()
        {
            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(_rowVersion, result.Data);
            Assert.AreEqual(_rowVersion, _mode.RowVersion.ConvertToString());
        }

        [TestMethod]
        public async Task HandlingVoidModeCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);

            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}
