using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.ModeCommands.UnvoidMode;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Test.Common.ExtensionMethods;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.ModeCommands.UnvoidMode
{
    [TestClass]
    public class UnvoidModeCommandHandlerTests : CommandHandlerTestsBase
    {
        private Mode _mode;
        private UnvoidModeCommand _command;
        private UnvoidModeCommandHandler _dut;
        private readonly string _rowVersion = "AAAAAAAAAAA=";
        private Mock<Mode> _modeMock;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            var modeId = 1;
            var modeRepositoryMock = new Mock<IModeRepository>();
            //_mode.SetProtectedIdForTesting(modeId);
            //modeRepositoryMock.Setup(m => m.GetByIdAsync(modeId))
            //    .Returns(Task.FromResult(_mode));

            _mode = new Mode(TestPlant, "ModeTitle");
            _modeMock = new Mock<Mode>(TestPlant, "ModeTitle");
            _modeMock.SetupGet(j => j.Plant).Returns(TestPlant);
            _modeMock.SetupGet(j => j.Id).Returns(modeId);
            modeRepositoryMock
                .Setup(r => r.GetByIdAsync(modeId))
                .Returns(Task.FromResult(_modeMock.Object));

            _command = new UnvoidModeCommand(modeId, _rowVersion);

            _dut = new UnvoidModeCommandHandler(
                modeRepositoryMock.Object,
                UnitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task HandlingUnvoidModeCommand_ShouldUnvoidMode()
        {
            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.IsFalse(_mode.IsVoided);
        }

        [TestMethod]
        public async Task HandlingUnvoidModeCommand_ShouldSetAndReturnRowVersion()
        {
            await _dut.Handle(_command, default);
            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(_rowVersion, result.Data);
            Assert.AreEqual(_rowVersion, _mode.RowVersion.ConvertToString());
        }

        [TestMethod]
        public async Task HandlingUnvoidModeCommand_ShouldSave()
        {
            await _dut.Handle(_command, default);

            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}
