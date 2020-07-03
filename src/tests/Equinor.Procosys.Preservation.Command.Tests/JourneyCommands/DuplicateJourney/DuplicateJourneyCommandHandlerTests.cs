using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.JourneyCommands.DuplicateJourney;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.JourneyCommands.DuplicateJourney
{
    [TestClass]
    public class DuplicateJourneyCommandHandlerTests : CommandHandlerTestsBase
    {
        private const string TestJourneyWithoutSteps = "TestJourneyWithoutSteps";
        private const string TestJourneyWithAStep = "TestJourneyWithAStep";
        private const string TestStep = "TestStep";
        private Journey _journeyAdded;
        private Mock<IJourneyRepository> _journeyRepositoryMock;
        private Mock<IModeRepository> _modeRepositoryMock;
        private Mock<IResponsibleRepository> _responsibleRepositoryMock;
        private DuplicateJourneyCommand _journeyWithoutStepsCommand;
        private DuplicateJourneyCommand _journeyWithAStepCommand;
        private DuplicateJourneyCommandHandler _dut;
        private readonly int _journeyWithoutStepsId = 3;
        private readonly int _journeyWithAStepId = 13;
        private readonly int _responsibleId = 4;
        private readonly int _modeId = 5;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            var mode = new Mode(TestPlant, "M", false);
            mode.SetProtectedIdForTesting(_modeId);
            _modeRepositoryMock = new Mock<IModeRepository>();
            _modeRepositoryMock
                .Setup(m => m.GetByIdsAsync(new List<int> {_modeId}))
                .Returns(Task.FromResult(new List<Mode> {mode}));
            
            var responsible = new Responsible(TestPlant, "RC", "RD");
            responsible.SetProtectedIdForTesting(_responsibleId);
            _responsibleRepositoryMock = new Mock<IResponsibleRepository>();
            _responsibleRepositoryMock
                .Setup(r => r.GetByIdsAsync(new List<int> {_responsibleId}))
                .Returns(Task.FromResult(new List<Responsible> {responsible}));
            
            var step = new Step(TestPlant, TestStep, mode, responsible);

            var sourceJourneyWithoutSteps = new Journey(TestPlant, TestJourneyWithoutSteps);
            var sourceJourneyWithAStep = new Journey(TestPlant, TestJourneyWithAStep);
            sourceJourneyWithAStep.AddStep(step);

            _journeyRepositoryMock = new Mock<IJourneyRepository>();
            _journeyRepositoryMock.Setup(j => j.GetByIdAsync(_journeyWithoutStepsId))
                .Returns(Task.FromResult(sourceJourneyWithoutSteps));
            _journeyRepositoryMock.Setup(j => j.GetByIdAsync(_journeyWithAStepId))
                .Returns(Task.FromResult(sourceJourneyWithAStep));
            _journeyRepositoryMock
                .Setup(repo => repo.Add(It.IsAny<Journey>()))
                .Callback<Journey>(journey =>
                {
                    _journeyAdded = journey;
                });

            _journeyWithoutStepsCommand = new DuplicateJourneyCommand(_journeyWithoutStepsId);
            _journeyWithAStepCommand = new DuplicateJourneyCommand(_journeyWithAStepId);

            _dut = new DuplicateJourneyCommandHandler(
                _journeyRepositoryMock.Object,
                _modeRepositoryMock.Object,
                _responsibleRepositoryMock.Object,
                UnitOfWorkMock.Object,
                PlantProviderMock.Object);
        }

        [TestMethod]
        public async Task HandlingDuplicateJourneyCommand_ShouldAddCopyOfSourceJourneyToRepository()
        {
            // Act
            var result = await _dut.Handle(_journeyWithoutStepsCommand, default);
            
            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(0, result.Data);
            Assert.IsNotNull(_journeyAdded);
        }

        [TestMethod]
        public async Task HandlingDuplicateJourneyCommand_ShouldCreateCopyOfSourceJourneyWithoutSteps()
        {
            // Act
            var result = await _dut.Handle(_journeyWithoutStepsCommand, default);
            
            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual($"{TestJourneyWithoutSteps}{Journey.DuplicatePrefix}", _journeyAdded.Title);
            Assert.AreEqual(0, _journeyAdded.Steps.Count);
        }

        [TestMethod]
        public async Task HandlingDuplicateJourneyCommand_ShouldCreateCopyOfSourceJourneyWithSteps()
        {
            // Act
            var result = await _dut.Handle(_journeyWithAStepCommand, default);
            
            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual($"{TestJourneyWithAStep}{Journey.DuplicatePrefix}", _journeyAdded.Title);
            Assert.AreEqual(1, _journeyAdded.Steps.Count);

            var step = _journeyAdded.Steps.Single();
            Assert.AreEqual(_modeId, step.ModeId);
            Assert.AreEqual(_responsibleId, step.ResponsibleId);
        }

        [TestMethod]
        public async Task HandlingDuplicateJourneyCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_journeyWithoutStepsCommand, default);
            
            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}
