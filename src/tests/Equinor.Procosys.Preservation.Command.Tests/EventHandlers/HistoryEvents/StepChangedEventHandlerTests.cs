using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.EventHandlers.HistoryEvents;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.HistoryAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.Domain.Events;
using Equinor.Procosys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.EventHandlers.HistoryEvents
{
    [TestClass]
    public class StepChangedEventHandlerTests
    {
        private readonly string TestPlant = "TestPlant";
        private readonly string Journey1 = "TestJourney1";
        private readonly string Journey2 = "TestJourney2";
        private readonly string FromStep = "TestStep1";
        private readonly string ToStepInJourney1 = "TestStep2";
        private readonly string ToStepInJourney2 = "TestStep3";
        private readonly int Journey1Id = 12;
        private readonly int Journey2Id = 13;
        private readonly int FromStepId = 1;
        private readonly int ToStepIdInJourney1 = 2;
        private readonly int ToStepIdInJourney2 = 3;
        private Mock<IHistoryRepository> _historyRepositoryMock;
        private StepChangedEventHandler _dut;
        private History _historyAdded;
        private Mock<IJourneyRepository> _journeyRepositoryMock;

        [TestInitialize]
        public void Setup()
        {
            var modeMock = new Mock<Mode>();
            modeMock.SetupGet(m => m.Plant).Returns(TestPlant);
            var respMock = new Mock<Responsible>();
            respMock.SetupGet(m => m.Plant).Returns(TestPlant);

            // Arrange
            var fromStep = new Step(TestPlant, FromStep, modeMock.Object, respMock.Object);
            fromStep.SetProtectedIdForTesting(FromStepId);
            var toStepInJourney1 = new Step(TestPlant, ToStepInJourney1, modeMock.Object, respMock.Object);
            toStepInJourney1.SetProtectedIdForTesting(ToStepIdInJourney1);
            var toStepInJourney2 = new Step(TestPlant, ToStepInJourney2, modeMock.Object, respMock.Object);
            toStepInJourney2.SetProtectedIdForTesting(ToStepIdInJourney2);

            var journey1 = new Journey(TestPlant, Journey1);
            journey1.SetProtectedIdForTesting(Journey1Id);
            journey1.AddStep(fromStep);
            journey1.AddStep(toStepInJourney1);

            var journey2 = new Journey(TestPlant, Journey2);
            journey2.SetProtectedIdForTesting(Journey2Id);
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
                .Setup(repo => repo.GetJourneysByStepIdsAsync(new List<int>{FromStepId, ToStepIdInJourney1}))
                .Returns(Task.FromResult(new List<Journey> { journey1, journey1 }));
            _journeyRepositoryMock
                .Setup(repo => repo.GetJourneysByStepIdsAsync(new List<int>{FromStepId, ToStepIdInJourney2}))
                .Returns(Task.FromResult(new List<Journey> { journey1, journey2 }));
            _dut = new StepChangedEventHandler(_historyRepositoryMock.Object, _journeyRepositoryMock.Object);
        }

        [TestMethod]
        public async Task Handle_ShouldAddStepChangedHistoryRecord_WhenStepsInSameJourney()
        {
            // Arrange
            Assert.IsNull(_historyAdded);

            // Act
            var objectGuid = Guid.NewGuid();
            await _dut.Handle(new StepChangedEvent(TestPlant, objectGuid, FromStepId, ToStepIdInJourney1), default);

            // Assert
            var expectedDescription = $"{EventType.StepChanged.GetDescription()} - From '{FromStep}' to '{ToStepInJourney1}' in journey '{Journey1}'";

            Assert.IsNotNull(_historyAdded);
            Assert.AreEqual(TestPlant, _historyAdded.Plant);
            Assert.AreEqual(objectGuid, _historyAdded.ObjectGuid);
            Assert.IsNotNull(_historyAdded.Description);
            Assert.AreEqual(EventType.StepChanged, _historyAdded.EventType);
            Assert.AreEqual(ObjectType.Tag, _historyAdded.ObjectType);
            Assert.AreEqual(expectedDescription, _historyAdded.Description);
            Assert.IsNull(_historyAdded.PreservationRecordId);
        }

        [TestMethod]
        public async Task Handle_ShouldAddJourneyChangedHistoryRecord_WhenStepsInDifferentJourneys()
        {
            // Arrange
            Assert.IsNull(_historyAdded);

            // Act
            var objectGuid = Guid.NewGuid();
            await _dut.Handle(new StepChangedEvent(TestPlant, objectGuid, FromStepId, ToStepIdInJourney2), default);

            // Assert
            var expectedDescription = $"{EventType.JourneyChanged.GetDescription()} - From '{FromStep}' in journey '{Journey1}' to '{ToStepInJourney2}' in journey '{Journey2}'";

            Assert.IsNotNull(_historyAdded);
            Assert.AreEqual(TestPlant, _historyAdded.Plant);
            Assert.AreEqual(objectGuid, _historyAdded.ObjectGuid);
            Assert.IsNotNull(_historyAdded.Description);
            Assert.AreEqual(EventType.JourneyChanged, _historyAdded.EventType);
            Assert.AreEqual(ObjectType.Tag, _historyAdded.ObjectType);
            Assert.AreEqual(expectedDescription, _historyAdded.Description);
            Assert.IsNull(_historyAdded.PreservationRecordId);
        }
    }
}
