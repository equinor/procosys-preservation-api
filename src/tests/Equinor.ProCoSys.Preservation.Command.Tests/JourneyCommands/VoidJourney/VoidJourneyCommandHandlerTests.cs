using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.JourneyCommands.VoidJourney;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.JourneyCommands.VoidJourney
{
    [TestClass]
    public class VoidJourneyCommandHandlerTests : CommandHandlerTestsBase
    {
        private Journey _journey;
        private VoidJourneyCommand _command;
        private VoidJourneyCommandHandler _dut;
        private readonly string _rowVersion = "AAAAAAAAABA=";

        [TestInitialize]
        public void Setup()
        {
            var journeyId = 2;
            var journeyRepositoryMock = new Mock<IJourneyRepository>();

            _journey = new Journey(TestPlant, "JourneyTitle");
            journeyRepositoryMock
                .Setup(r => r.GetByIdAsync(journeyId))
                .Returns(Task.FromResult(_journey));

            _command = new VoidJourneyCommand(journeyId, _rowVersion);

            _dut = new VoidJourneyCommandHandler(
                journeyRepositoryMock.Object,
                UnitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task HandlingVoidJourneyCommand_ShouldVoidJourney()
        {
            // Arrange
            Assert.IsFalse(_journey.IsVoided);

            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.IsTrue(_journey.IsVoided);
        }

        [TestMethod]
        public async Task HandlingVoidJourneyCommand_ShouldSetAndReturnRowVersion()
        {
            // Act
            var result =  await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(_rowVersion, result.Data);
            Assert.AreEqual(_rowVersion, _journey.RowVersion.ConvertToString());
        }

        [TestMethod]
        public async Task HandlingVoidJourneyCommand_ShouldSave()
        {
            await _dut.Handle(_command, default);
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}
