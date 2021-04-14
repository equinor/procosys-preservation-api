using System;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.HistoryEvents;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.HistoryAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.HistoryEvents
{
    [TestClass]
    public class TagCreatedEventHandlerTests
    {
        private Mock<IHistoryRepository> _historyRepositoryMock;
        private TagCreatedEventHandler _dut;
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

            _dut = new TagCreatedEventHandler(_historyRepositoryMock.Object);
        }

        [TestMethod]
        public void Handle_ShouldAddTagCreatedHistoryRecord()
        {
            // Arrange
            Assert.IsNull(_historyAdded);
            
            // Act
            var objectGuid = Guid.NewGuid();
            var plant = "TestPlant";
            _dut.Handle(new TagCreatedEvent(plant, objectGuid), default);

            // Assert
            Assert.IsNotNull(_historyAdded);
            Assert.AreEqual(plant,_historyAdded.Plant);
            Assert.AreEqual(objectGuid,_historyAdded.ObjectGuid);
            Assert.IsNotNull(_historyAdded.Description);
            Assert.AreEqual(EventType.TagCreated,_historyAdded.EventType);
            Assert.AreEqual(ObjectType.Tag,_historyAdded.ObjectType);
            Assert.IsFalse(_historyAdded.PreservationRecordGuid.HasValue);
            Assert.IsFalse(_historyAdded.DueInWeeks.HasValue);
        }
    }
}
