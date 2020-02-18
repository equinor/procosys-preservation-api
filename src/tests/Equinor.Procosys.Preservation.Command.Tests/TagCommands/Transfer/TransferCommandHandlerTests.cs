using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.TagCommands.Transfer;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.Transfer
{
    [TestClass]
    public class TransferCommandHandlerTests : CommandHandlerTestsBase
    {
        private const int TagId1 = 7;
        private const int TagId2 = 8;
        private const int TwoWeeksInterval = 2;
        private const int FourWeeksInterval = 4;

        private Mock<IProjectRepository> _projectRepoMock;
        private TransferCommand _command;

        private TransferCommandHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            _dut = new TransferCommandHandler(_projectRepoMock.Object, UnitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task HandlingTransferCommand_Should()
        {
            await _dut.Handle(_command, default);
        }

        [TestMethod]
        public async Task HandlingTransferCommand_ShouldSave()
        {
            await _dut.Handle(_command, default);

            UnitOfWorkMock.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }
    }
}
