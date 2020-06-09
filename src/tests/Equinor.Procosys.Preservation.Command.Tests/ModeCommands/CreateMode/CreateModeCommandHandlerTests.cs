using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.ModeCommands.CreateMode;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.ModeCommands.CreateMode
{
    [TestClass]
    public class CreateModeCommandHandlerTests : CommandHandlerTestsBase
    {
        private const string TestMode = "TestMode";
        private Mock<IModeRepository> _modeRepositoryMock;
        private Mode _modeAdded;
        private CreateModeCommand _command;
        private CreateModeCommandHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            _modeRepositoryMock = new Mock<IModeRepository>();
            _modeRepositoryMock
                .Setup(x => x.Add(It.IsAny<Mode>()))
                .Callback<Mode>(x =>
                {
                    _modeAdded = x;
                });

            _command = new CreateModeCommand(TestMode, false, TestUserOid);

            _dut = new CreateModeCommandHandler(
                _modeRepositoryMock.Object,
                UnitOfWorkMock.Object,
                PlantProviderMock.Object
                );
        }

        
        [TestMethod]
        public async Task HandlingCreateModeCommand_ShouldAddModeToRepository()
        {
            // Act
            var result = await _dut.Handle(_command, default);
            
            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(0, result.Data);
            Assert.AreEqual(0, _modeAdded.Id);
            Assert.AreEqual(TestMode, _modeAdded.Title);
            Assert.IsFalse(_modeAdded.ForSupplier);
        }

        [TestMethod]
        public async Task HandlingCreateModeCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);
            
            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(_command.CurrentUserOid, default), Times.Once);
        }
    }
}
