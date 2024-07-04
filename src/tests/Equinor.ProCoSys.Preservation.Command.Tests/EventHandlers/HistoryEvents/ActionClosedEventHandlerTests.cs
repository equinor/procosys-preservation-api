using System;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.HistoryEvents;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.HistoryAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.HistoryEvents
{
    [TestClass]
    public class ActionClosedEventHandlerTests
    {
        private Mock<IHistoryRepository> _historyRepositoryMock;
        private Mock<IProjectRepository> _projectRepository;
        private ActionClosedEventHandler _dut;
        private History _historyAdded;
        private Guid _tagGuid;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            _tagGuid = Guid.NewGuid();

            var mockTag = new Mock<Tag>().Object;
            mockTag.Guid = _tagGuid;

            _historyRepositoryMock = new Mock<IHistoryRepository>();
            _historyRepositoryMock
                .Setup(repo => repo.Add(It.IsAny<History>()))
                .Callback<History>(history =>
                {
                    _historyAdded = history;
                });
            _projectRepository = new Mock<IProjectRepository>();
            _projectRepository
                .Setup(p => p.GetTagOnlyByTagIdAsync(It.IsAny<int>()))
                .ReturnsAsync(mockTag);

            _dut = new ActionClosedEventHandler(_historyRepositoryMock.Object, _projectRepository.Object, null);
        }

        [TestMethod]
        public void Handle_ShouldAddActionClosedHistoryRecord()
        {
            // Arrange
            Assert.IsNull(_historyAdded);

            // Act
            var plant = "TestPlant";
            _dut.Handle(new ActionClosedEvent(plant, new Action(plant, "title", "", null)), default);

            // Assert
            Assert.IsNotNull(_historyAdded);
            Assert.AreEqual(plant, _historyAdded.Plant);
            Assert.AreEqual(_tagGuid, _historyAdded.SourceGuid);
            Assert.IsNotNull(_historyAdded.Description);
            Assert.AreEqual(EventType.ActionClosed, _historyAdded.EventType);
            Assert.AreEqual(ObjectType.Tag, _historyAdded.ObjectType);
            Assert.IsFalse(_historyAdded.PreservationRecordGuid.HasValue);
            Assert.IsFalse(_historyAdded.DueInWeeks.HasValue);
        }
    }
}
