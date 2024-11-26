using System;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.HistoryEvents;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.HistoryAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.HistoryEvents
{
    [TestClass]
    public class ActionAddedEventHandlerTests
    {
        private Mock<IHistoryRepository> _historyRepositoryMock;
        private Mock<IProjectRepository> _projectRepository;
        private ActionAddedEventHandler _dut;
        private History _historyAdded;
        private Tag _mockTag;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            _mockTag = new Mock<Tag>().Object;
            _mockTag.Guid = Guid.NewGuid();

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
                .ReturnsAsync(_mockTag);

            _projectRepository
                .Setup(p => p.GetTagByActionGuidAsync(It.IsAny<Guid>()))
                .ReturnsAsync(_mockTag);

            _dut = new ActionAddedEventHandler(_historyRepositoryMock.Object);
        }

        [TestMethod]
        public void Handle_ShouldAddActionAddedHistoryRecord()
        {
            // Arrange
            Assert.IsNull(_historyAdded);

            // Act
            var plant = "TestPlant";
            var title = "Action1";

            _dut.Handle(new ChildAddedEvent<Tag, Action>(_mockTag, new Action(plant, title, "", null)), default);

            // Assert
            var expectedDescription = $"{EventType.ActionAdded.GetDescription()} - '{title}'";

            Assert.IsNotNull(_historyAdded);
            Assert.AreEqual(plant, _historyAdded.Plant);
            Assert.AreEqual(_mockTag.Guid, _historyAdded.SourceGuid);
            Assert.IsNotNull(_historyAdded.Description);
            Assert.AreEqual(EventType.ActionAdded, _historyAdded.EventType);
            Assert.AreEqual(ObjectType.Tag, _historyAdded.ObjectType);
            Assert.AreEqual(expectedDescription, _historyAdded.Description);
            Assert.IsFalse(_historyAdded.PreservationRecordGuid.HasValue);
            Assert.IsFalse(_historyAdded.DueInWeeks.HasValue);
        }
    }
}
