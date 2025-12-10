using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Command.ModeCommands.UpdateMode;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.ModeCommands.UpdateMode
{
    [TestClass]
    public class UpdateModeCommandHandlerTests : CommandHandlerTestsBase
    {
        private readonly string _oldTitle = "ModeTitleOld";
        private readonly string _newTitle = "ModeTitleNew";
        private readonly string _rowVersion = "AAAAAAAAABA=";

        private UpdateModeCommand _command;
        private UpdateModeCommandHandler _dut;
        private Mode _mode;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            var modeId = 1;
            var modeRepositoryMock = new Mock<IModeRepository>();
            _mode = new Mode(TestPlant, _oldTitle, false);
            modeRepositoryMock.Setup(m => m.GetByIdAsync(modeId))
                .Returns(Task.FromResult(_mode));
            _command = new UpdateModeCommand(modeId, _newTitle, true, _rowVersion);

            _dut = new UpdateModeCommandHandler(
                modeRepositoryMock.Object,
                UnitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task HandlingUpdateModeCommand_ShouldUpdateMode()
        {
            // Arrange
            Assert.AreEqual(_oldTitle, _mode.Title);
            Assert.IsFalse(_mode.ForSupplier);

            // Act
            await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(_newTitle, _mode.Title);
            Assert.IsTrue(_mode.ForSupplier);

        }

        [TestMethod]
        public async Task HandlingUpdateModeCommand_ShouldSetAndReturnRowVersion()
        {
            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            // In real life EF Core will create a new RowVersion when save.
            // Since UnitOfWorkMock is a Mock this will not happen here, so we assert that RowVersion is set from command
            Assert.AreEqual(_rowVersion, result.Data);
            Assert.AreEqual(_rowVersion, _mode.RowVersion.ConvertToString());
        }

        [TestMethod]
        public async Task HandlingUpdateModeCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);

            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}
