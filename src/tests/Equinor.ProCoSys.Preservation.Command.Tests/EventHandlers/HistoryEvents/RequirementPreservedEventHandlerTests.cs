using System;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.HistoryEvents;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.HistoryAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.HistoryEvents
{
    [TestClass]
    public class RequirementPreservedEventHandlerTests
    {
        private const int _requirementDefinitionId = 3;
        private const string _plant = "TestPlant";

        private Mock<IRequirementTypeRepository> _requirementTypeRepositoryMock;
        private Mock<IHistoryRepository> _historyRepositoryMock;
        private TagRequirementPreservedEventHandler _dut;
        private History _historyAdded;
        private RequirementDefinition _requirementDefinition;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            _requirementDefinition = new RequirementDefinition(_plant, "Rotate 2 turns", 2, RequirementUsage.ForAll, 1);

            _historyRepositoryMock = new Mock<IHistoryRepository>();
            _historyRepositoryMock
                .Setup(repo => repo.Add(It.IsAny<History>()))
                .Callback<History>(history =>
                {
                    _historyAdded = history;
                });

            _requirementTypeRepositoryMock = new Mock<IRequirementTypeRepository>();
            _requirementTypeRepositoryMock
                .Setup(repo => repo.GetRequirementDefinitionByIdAsync(_requirementDefinitionId))
                .Returns(Task.FromResult(_requirementDefinition));

            _dut = new TagRequirementPreservedEventHandler(_historyRepositoryMock.Object, _requirementTypeRepositoryMock.Object);
        }

        [TestMethod]
        public void Handle_ShouldAddRequirementPreservedHistoryRecord()
        {
            // Arrange
            Assert.IsNull(_historyAdded);

            // Act
            var sourceGuid = Guid.NewGuid();
            var preservationRecordGuid = Guid.NewGuid();
            var dueInWeeks = 2;

            var tagRequirement = new Mock<TagRequirement>().Object;
            tagRequirement.RequirementDefinitionId = _requirementDefinitionId;

            _dut.Handle(new TagRequirementPreservedEvent(_plant, sourceGuid, tagRequirement, dueInWeeks, preservationRecordGuid), default);

            // Assert
            var expectedDescription = $"{EventType.RequirementPreserved.GetDescription()} - '{_requirementDefinition.Title}'";

            Assert.IsNotNull(_historyAdded);
            Assert.AreEqual(_plant, _historyAdded.Plant);
            Assert.AreEqual(sourceGuid, _historyAdded.SourceGuid);
            Assert.IsNotNull(_historyAdded.Description);
            Assert.AreEqual(EventType.RequirementPreserved, _historyAdded.EventType);
            Assert.AreEqual(ObjectType.Tag, _historyAdded.ObjectType);
            Assert.AreEqual(expectedDescription, _historyAdded.Description);
            Assert.IsTrue(_historyAdded.PreservationRecordGuid.HasValue);
            Assert.IsTrue(_historyAdded.DueInWeeks.HasValue);
            Assert.AreEqual(preservationRecordGuid, _historyAdded.PreservationRecordGuid.Value);
            Assert.AreEqual(dueInWeeks, _historyAdded.DueInWeeks.Value);
        }
    }
}
