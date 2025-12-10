using System;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.HistoryEvents;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.HistoryAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.HistoryEvents
{
    [TestClass]
    public class IntervalChangedEventHandlerTests
    {
        private const int RequirementDefinitionId = 3;
        private Mock<IHistoryRepository> _historyRepositoryMock;
        private IntervalChangedEventHandler _dut;
        private History _historyAdded;
        private RequirementDefinition _requirementDefinition;
        private Mock<IRequirementTypeRepository> _requirementTypeRepositoryMock;
        private string _reqTitle;
        private const string Plant = "TestPlant";

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            _reqTitle = "Rotate 2 turns";
            _requirementDefinition = new RequirementDefinition(Plant, _reqTitle, 2, RequirementUsage.ForAll, 1);
            _requirementTypeRepositoryMock = new Mock<IRequirementTypeRepository>();
            _requirementTypeRepositoryMock
                .Setup(repo => repo.GetRequirementDefinitionByIdAsync(RequirementDefinitionId))
                .Returns(Task.FromResult(_requirementDefinition));

            _historyRepositoryMock = new Mock<IHistoryRepository>();
            _historyRepositoryMock
                .Setup(repo => repo.Add(It.IsAny<History>()))
                .Callback<History>(history =>
                {
                    _historyAdded = history;
                });

            _dut = new IntervalChangedEventHandler(_historyRepositoryMock.Object, _requirementTypeRepositoryMock.Object);
        }

        [TestMethod]
        public void Handle_ShouldAddIntervalChangedHistoryRecord()
        {
            // Arrange
            Assert.IsNull(_historyAdded);

            // Act
            var sourceGuid = Guid.NewGuid();
            var plant = "TestPlant";
            var fromInterval = 1;
            var toInterval = 2;
            _dut.Handle(new IntervalChangedEvent(plant, sourceGuid, RequirementDefinitionId, fromInterval, toInterval), default);

            // Assert
            var expectedDescription = $"{_historyAdded?.EventType.GetDescription()} - From {fromInterval} week(s) to {toInterval} week(s) in '{_reqTitle}'";

            Assert.IsNotNull(_historyAdded);
            Assert.AreEqual(plant, _historyAdded.Plant);
            Assert.AreEqual(sourceGuid, _historyAdded.SourceGuid);
            Assert.IsNotNull(_historyAdded.Description);
            Assert.AreEqual(EventType.IntervalChanged, _historyAdded.EventType);
            Assert.AreEqual(ObjectType.Tag, _historyAdded.ObjectType);
            Assert.AreEqual(expectedDescription, _historyAdded.Description);
            Assert.IsFalse(_historyAdded.PreservationRecordGuid.HasValue);
            Assert.IsFalse(_historyAdded.DueInWeeks.HasValue);
        }
    }
}
