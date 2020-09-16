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
        private const string TestJourneyWith3Steps = "TestJourneyWith3Steps";
        private Journey _journeyAdded;
        private Mock<IJourneyRepository> _journeyRepositoryMock;
        private Mock<IModeRepository> _modeRepositoryMock;
        private Mock<IResponsibleRepository> _responsibleRepositoryMock;
        private DuplicateJourneyCommand _journeyWithoutStepsCommand;
        private DuplicateJourneyCommand _journeyWith3StepsCommand;
        private DuplicateJourneyCommandHandler _dut;
        private readonly int _journeyWithoutStepsId = 3;
        private readonly int _journeyWith3StepsId = 13;
        private readonly int _responsibleId = 4;
        private readonly int _modeId = 5;
        private readonly int _stepAId = 15;
        private readonly int _stepBId = 16;
        private readonly int _stepCId = 17;
        private readonly string _stepA = "TestStepA";
        private readonly string _stepB = "TestStepB";
        private readonly string _stepC = "TestStepC";
        private Journey _sourceJourneyWith3Steps;

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

            var stepA = new Step(TestPlant, _stepA, mode, responsible);
            stepA.SetProtectedIdForTesting(_stepAId);
            var stepB = new Step(TestPlant, _stepB, mode, responsible);
            stepB.SetProtectedIdForTesting(_stepBId);
            var stepC = new Step(TestPlant, _stepC, mode, responsible);
            stepC.SetProtectedIdForTesting(_stepCId);

            var sourceJourneyWithoutSteps = new Journey(TestPlant, TestJourneyWithoutSteps);
            _sourceJourneyWith3Steps = new Journey(TestPlant, TestJourneyWith3Steps);
            _sourceJourneyWith3Steps.AddStep(stepA);
            _sourceJourneyWith3Steps.AddStep(stepB);
            _sourceJourneyWith3Steps.AddStep(stepC);

            _journeyRepositoryMock = new Mock<IJourneyRepository>();
            _journeyRepositoryMock.Setup(j => j.GetByIdAsync(_journeyWithoutStepsId))
                .Returns(Task.FromResult(sourceJourneyWithoutSteps));
            _journeyRepositoryMock.Setup(j => j.GetByIdAsync(_journeyWith3StepsId))
                .Returns(Task.FromResult(_sourceJourneyWith3Steps));
            _journeyRepositoryMock
                .Setup(repo => repo.Add(It.IsAny<Journey>()))
                .Callback<Journey>(journey =>
                {
                    _journeyAdded = journey;
                });

            _journeyWithoutStepsCommand = new DuplicateJourneyCommand(_journeyWithoutStepsId);
            _journeyWith3StepsCommand = new DuplicateJourneyCommand(_journeyWith3StepsId);

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
            var result = await _dut.Handle(_journeyWith3StepsCommand, default);
            
            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual($"{TestJourneyWith3Steps}{Journey.DuplicatePrefix}", _journeyAdded.Title);
            Assert.AreEqual(3, _journeyAdded.Steps.Count);

            AssertStep(_journeyAdded.Steps.ElementAt(0), _stepA);
            AssertStep(_journeyAdded.Steps.ElementAt(1), _stepB);
            AssertStep(_journeyAdded.Steps.ElementAt(2), _stepC);
        }

        [TestMethod]
        public async Task HandlingDuplicateJourneyCommand_ShouldCreateCopyOfSourceJourneyWithStepsInCorrectOrder()
        {
            // Arrange
            _sourceJourneyWith3Steps.SwapSteps(_stepBId, _stepCId);
            
            // Act
            var result = await _dut.Handle(_journeyWith3StepsCommand, default);
            
            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual($"{TestJourneyWith3Steps}{Journey.DuplicatePrefix}", _journeyAdded.Title);
            Assert.AreEqual(3, _journeyAdded.Steps.Count);

            AssertStep(_journeyAdded.Steps.ElementAt(0), _stepA);
            AssertStep(_journeyAdded.Steps.ElementAt(1), _stepC);
            AssertStep(_journeyAdded.Steps.ElementAt(2), _stepB);
        }

        [TestMethod]
        public async Task HandlingDuplicateJourneyCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_journeyWithoutStepsCommand, default);
            
            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        private void AssertStep(Step step, string stepTitle)
        {
            Assert.AreEqual(_modeId, step.ModeId);
            Assert.AreEqual(_responsibleId, step.ResponsibleId);
            Assert.AreEqual(stepTitle, step.Title);
        }
    }
}
