using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.TagCommands.Transfer;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.Transfer
{
    [TestClass]
    public class TransferCommandHandlerTests : CommandHandlerTestsBase
    {
        private const int StepId1OnJourney1 = 1;
        private const int StepId2OnJourney1 = 2;
        private const int StepId1OnJourney2 = 3;
        private const int StepId2OnJourney2 = 4;

        private const int TagId1 = 7;
        private const int TagId2 = 8;

        private TransferCommand _command;

        private TransferCommandHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            var journeyRepoMock = new Mock<IJourneyRepository>();



            var projectRepoMock = new Mock<IProjectRepository>();
            _dut = new TransferCommandHandler(projectRepoMock.Object, journeyRepoMock.Object, UnitOfWorkMock.Object);
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
