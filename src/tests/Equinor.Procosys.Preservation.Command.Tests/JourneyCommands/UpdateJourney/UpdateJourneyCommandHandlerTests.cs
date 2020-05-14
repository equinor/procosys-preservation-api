using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.JourneyCommands.UpdateJourney;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Test.Common.ExtensionMethods;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.JourneyCommands.UpdateJourney
{
    [TestClass]
    public class UpdateJourneyCommandHandlerTests : CommandHandlerTestsBase
    {
        private readonly string _oldTitle = "JourneyTitleOld";
        private readonly string _newTitle = "JourneyTitleNew";
        private readonly string _rowVersion = "AAAAAAAAABA=";

        private UpdateJourneyCommand _command;
        private UpdateJourneyCommandHandler _dut;
        private Journey _journey;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            var testJourneyId = 1;
            var journeyRepositoryMock = new Mock<IJourneyRepository>();
            _journey = new Journey(TestPlant, _oldTitle);
            _journey.SetProtectedIdForTesting(testJourneyId);
            journeyRepositoryMock.Setup(j => j.GetByIdAsync(testJourneyId))
                .Returns(Task.FromResult(_journey));
            _command = new UpdateJourneyCommand(testJourneyId, _newTitle, _rowVersion);

            _dut = new UpdateJourneyCommandHandler(
                journeyRepositoryMock.Object,
                UnitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task HandlingUpdateJourneyCommand_ShouldUpdateJourney()
        {
            // Arrange
            Assert.AreEqual(_oldTitle, _journey.Title);

            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(_newTitle, _journey.Title);
        }

        [TestMethod]
        public async Task HandlingUpdateJourneyCommand_ShouldSetAndReturnRowVersion()
        {
            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            // In real life EF Core will create a new RowVersion when save.
            // Since UnitOfWorkMock is a Mock this will not happen here, so we assert that RowVersion is set from command
            Assert.AreEqual(_rowVersion, result.Data);
            Assert.AreEqual(_rowVersion, _journey.RowVersion.ConvertToString());
        }

        [TestMethod]
        public async Task HandlingUpdateJourneyCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);

            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}
