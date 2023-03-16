using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.JourneyCommands.SwapSteps;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.ProCoSys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.JourneyCommands.SwapSteps
{
    [TestClass]
    public class SwapStepsCommandHandlerTests : CommandHandlerTestsBase
    {
        private readonly string _rowVersionA = "AAAAAAAAABA=";
        private readonly string _rowVersionB = "AAAAAAAACBA=";

        private readonly string _stepATitle = "StepATitle";
        private readonly string _stepBTitle = "StepBTitle";
        private SwapStepsCommand _command;
        private SwapStepsCommandHandler _dut;
        private Step _stepA;
        private Step _stepB;
        private int _sortKeyA;
        private int _sortKeyB;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            var journeyRepositoryMock = new Mock<IJourneyRepository>();

            var modeMock = new Mock<Mode>();
            modeMock.SetupGet(s => s.Plant).Returns(TestPlant);
            var responsibleMock = new Mock<Responsible>();
            responsibleMock.SetupGet(s => s.Plant).Returns(TestPlant);

            var journeyId = 1;
            var stepAId = 2;
            var stepBId = 3;

            var journey = new Journey(TestPlant, "TestJourney");
            journey.SetProtectedIdForTesting(journeyId);

            _stepA = new Step(TestPlant, _stepATitle, modeMock.Object, responsibleMock.Object);
            _stepA.SetProtectedIdForTesting(stepAId);
 
            journey.AddStep(_stepA);
            _sortKeyA = _stepA.SortKey;

            _stepB = new Step(TestPlant, _stepBTitle, modeMock.Object, responsibleMock.Object);
            _stepB.SetProtectedIdForTesting(stepBId);

            journey.AddStep(_stepB);
            _sortKeyB = _stepB.SortKey;

            journeyRepositoryMock.Setup(s => s.GetByIdAsync(journeyId))
                .Returns(Task.FromResult(journey));

            _command = new SwapStepsCommand(journeyId, stepAId, _rowVersionA, stepBId, _rowVersionB);

            _dut = new SwapStepsCommandHandler(
                journeyRepositoryMock.Object,
                UnitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task HandlingSwapStepsCommand_ShouldSwapStepsSortKeys()
        {
            // Arrange
            Assert.AreEqual(_sortKeyA, _stepA.SortKey);
            Assert.AreEqual(_sortKeyB, _stepB.SortKey);

            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(_sortKeyB, _stepA.SortKey);
            Assert.AreEqual(_sortKeyA, _stepB.SortKey);
        }
        
        [TestMethod]
        public async Task HandlingSwapStepsCommand_ShouldSetAndReturnRowVersion()
        {
            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            // In real life EF Core will create a new RowVersion when save.
            // Since UnitOfWorkMock is a Mock this will not happen here, so we assert that RowVersion is set from command
            Assert.AreEqual(_rowVersionA, result.Data.First().RowVersion);
            Assert.AreEqual(_rowVersionA, _stepA.RowVersion.ConvertToString());

            Assert.AreEqual(_rowVersionB, result.Data.Skip(1).First().RowVersion);
            Assert.AreEqual(_rowVersionB, _stepB.RowVersion.ConvertToString());
        }

        [TestMethod]
        public async Task HandlingSwapStepsCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);

            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}
