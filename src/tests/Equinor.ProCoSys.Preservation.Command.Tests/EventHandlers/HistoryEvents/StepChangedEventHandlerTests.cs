using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.HistoryEvents;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.HistoryAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.HistoryEvents
{
    [TestClass]
    public class StepChangedEventHandlerTests
    {
        private readonly string _testPlant = "TestPlant";
        private readonly string _journey1 = "TestJourney1";
        private readonly string _journey2 = "TestJourney2";
        private readonly string _fromStep = "TestStep1";
        private readonly string _toStepInJourney1 = "TestStep2";
        private readonly string _toStepInJourney2 = "TestStep3";
        private readonly int _journey1Id = 12;
        private readonly int _journey2Id = 13;
        private readonly int _fromStepId = 1;
        private readonly int _toStepIdInJourney1 = 2;
        private readonly int _toStepIdInJourney2 = 3;
        private Mock<IHistoryRepository> _historyRepositoryMock;
        private StepChangedEventHandler _dut;
        private History _historyAdded;
        private Mock<IJourneyRepository> _journeyRepositoryMock;

        [TestInitialize]
        public void Setup()
        {
            var modeMock = new Mock<Mode>();
            modeMock.SetupGet(m => m.Plant).Returns(_testPlant);
            var respMock = new Mock<Responsible>();
            respMock.SetupGet(m => m.Plant).Returns(_testPlant);

            // Arrange
            var fromStep = new Step(_testPlant, _fromStep, modeMock.Object, respMock.Object);
            fromStep.SetProtectedIdForTesting(_fromStepId);
            var toStepInJourney1 = new Step(_testPlant, _toStepInJourney1, modeMock.Object, respMock.Object);
            toStepInJourney1.SetProtectedIdForTesting(_toStepIdInJourney1);
            var toStepInJourney2 = new Step(_testPlant, _toStepInJourney2, modeMock.Object, respMock.Object);
            toStepInJourney2.SetProtectedIdForTesting(_toStepIdInJourney2);

            var journey1 = new Journey(_testPlant, _journey1);
            journey1.SetProtectedIdForTesting(_journey1Id);
            journey1.AddStep(fromStep);
            journey1.AddStep(toStepInJourney1);

            var journey2 = new Journey(_testPlant, _journey2);
            journey2.SetProtectedIdForTesting(_journey2Id);
            journey2.AddStep(toStepInJourney2);

            _historyRepositoryMock = new Mock<IHistoryRepository>();
            _historyRepositoryMock
                .Setup(repo => repo.Add(It.IsAny<History>()))
                .Callback<History>(history =>
                {
                    _historyAdded = history;
                });

            _journeyRepositoryMock = new Mock<IJourneyRepository>();
            _journeyRepositoryMock
                .Setup(repo => repo.GetJourneysByStepIdsAsync(new List<int> { _fromStepId, _toStepIdInJourney1 }))
                .Returns(Task.FromResult(new List<Journey> { journey1, journey1 }));
            _journeyRepositoryMock
                .Setup(repo => repo.GetJourneysByStepIdsAsync(new List<int> { _fromStepId, _toStepIdInJourney2 }))
                .Returns(Task.FromResult(new List<Journey> { journey1, journey2 }));
            _dut = new StepChangedEventHandler(_historyRepositoryMock.Object, _journeyRepositoryMock.Object);
        }

        [TestMethod]
        public async Task Handle_ShouldAddStepChangedHistoryRecord_WhenStepsInSameJourney()
        {
            // Arrange
            Assert.IsNull(_historyAdded);

            // Act
            var sourceGuid = Guid.NewGuid();
            await _dut.Handle(new StepChangedEvent(_testPlant, sourceGuid, _fromStepId, _toStepIdInJourney1), default);

            // Assert
            var expectedDescription = $"{EventType.StepChanged.GetDescription()} - From '{_fromStep}' to '{_toStepInJourney1}' in journey '{_journey1}'";

            Assert.IsNotNull(_historyAdded);
            Assert.AreEqual(_testPlant, _historyAdded.Plant);
            Assert.AreEqual(sourceGuid, _historyAdded.SourceGuid);
            Assert.IsNotNull(_historyAdded.Description);
            Assert.AreEqual(EventType.StepChanged, _historyAdded.EventType);
            Assert.AreEqual(ObjectType.Tag, _historyAdded.ObjectType);
            Assert.AreEqual(expectedDescription, _historyAdded.Description);
            Assert.IsFalse(_historyAdded.PreservationRecordGuid.HasValue);
            Assert.IsFalse(_historyAdded.DueInWeeks.HasValue);
        }

        [TestMethod]
        public async Task Handle_ShouldAddJourneyChangedHistoryRecord_WhenStepsInDifferentJourneys()
        {
            // Arrange
            Assert.IsNull(_historyAdded);

            // Act
            var sourceGuid = Guid.NewGuid();
            await _dut.Handle(new StepChangedEvent(_testPlant, sourceGuid, _fromStepId, _toStepIdInJourney2), default);

            // Assert
            var expectedDescription = $"{EventType.JourneyChanged.GetDescription()} - From journey '{_journey1}' / step '{_fromStep}' to journey '{_journey2}' / step '{_toStepInJourney2}'";

            Assert.IsNotNull(_historyAdded);
            Assert.AreEqual(_testPlant, _historyAdded.Plant);
            Assert.AreEqual(sourceGuid, _historyAdded.SourceGuid);
            Assert.IsNotNull(_historyAdded.Description);
            Assert.AreEqual(EventType.JourneyChanged, _historyAdded.EventType);
            Assert.AreEqual(ObjectType.Tag, _historyAdded.ObjectType);
            Assert.AreEqual(expectedDescription, _historyAdded.Description);
            Assert.IsFalse(_historyAdded.PreservationRecordGuid.HasValue);
            Assert.IsFalse(_historyAdded.DueInWeeks.HasValue);
        }
    }
}
