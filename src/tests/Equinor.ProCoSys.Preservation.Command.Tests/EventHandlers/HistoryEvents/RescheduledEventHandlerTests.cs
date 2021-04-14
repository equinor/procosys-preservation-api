using System;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.HistoryEvents;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.HistoryAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.HistoryEvents
{
    [TestClass]
    public class RescheduledEventHandlerTests
    {
        private Mock<IHistoryRepository> _historyRepositoryMock;
        private RescheduledEventHandler _dut;
        private History _historyAdded;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            _historyRepositoryMock = new Mock<IHistoryRepository>();
            _historyRepositoryMock
                .Setup(repo => repo.Add(It.IsAny<History>()))
                .Callback<History>(history =>
                {
                    _historyAdded = history;
                });

            _dut = new RescheduledEventHandler(_historyRepositoryMock.Object);
        }

        [TestMethod]
        public void Handle_ShouldAddRescheduledHistoryRecord()
        {
            // Arrange
            Assert.IsNull(_historyAdded);

            // Act
            var objectGuid = Guid.NewGuid();
            var plant = "TestPlant";
            var direction = RescheduledDirection.Earlier;
            var weeks = 2;
            var comment = "TestComment";
            _dut.Handle(new RescheduledEvent(plant, objectGuid, weeks, direction, comment), default);

            // Assert
            var expectedDescription = $"{EventType.Rescheduled.GetDescription()} - {weeks} week(s) {direction.ToString().ToLower()}. {comment}";

            Assert.IsNotNull(_historyAdded);
            Assert.AreEqual(plant, _historyAdded.Plant);
            Assert.AreEqual(objectGuid, _historyAdded.ObjectGuid);
            Assert.IsNotNull(_historyAdded.Description);
            Assert.AreEqual(EventType.Rescheduled, _historyAdded.EventType);
            Assert.AreEqual(ObjectType.Tag, _historyAdded.ObjectType);
            Assert.AreEqual(expectedDescription, _historyAdded.Description);
            Assert.IsFalse(_historyAdded.PreservationRecordGuid.HasValue);
            Assert.IsFalse(_historyAdded.DueInWeeks.HasValue);
        }
    }
}
