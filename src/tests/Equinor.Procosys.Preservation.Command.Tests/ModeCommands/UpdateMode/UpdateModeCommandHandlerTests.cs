using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.ModeCommands.UpdateMode;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Test.Common.ExtensionMethods;
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
        private readonly string _rowVersion = "AAAAAAAAABA=";

        private UpdateModeCommand _command;
        private UpdateModeCommandHandler _dut;
        private Mode _mode;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            var testModeId = 1;
            var modeRepositoryMock = new Mock<IModeRepository>();
            _mode = new Mode(TestPlant, _oldTitle);
            _mode.SetProtectedIdForTesting(testModeId);
            modeRepositoryMock.Setup(m => m.GetByIdAsync(testModeId))
                .Returns(Task.FromResult(_mode));
            _command = new UpdateModeCommand(testModeId, _newTitle, _rowVersion);

            _dut = new UpdateModeCommandHandler(
                modeRepositoryMock.Object,
                UnitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task HandlingUpdateModeCommand_ShouldUpdateMode()
        {
            // Arrange
            Assert.AreEqual(_oldTitle, _mode.Title);

            // Act
            await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(_mode.Title, _newTitle);
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
