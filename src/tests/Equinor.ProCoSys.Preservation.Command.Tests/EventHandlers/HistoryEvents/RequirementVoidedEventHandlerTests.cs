using System;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.HistoryEvents;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.HistoryAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.HistoryEvents
{
    [TestClass]
    public class RequirementVoidedEventHandlerTests
    {
        private const int _requirementDefinitionId = 3;
        private const string _plant = "TestPlant";

        private Mock<IRequirementTypeRepository> _requirementTypeRepositoryMock;
        private Mock<IHistoryRepository> _historyRepositoryMock;
        private TagRequirementVoidedEventHandler _dut;
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

            _dut = new TagRequirementVoidedEventHandler(_historyRepositoryMock.Object, _requirementTypeRepositoryMock.Object);
        }

        [TestMethod]
        public void Handle_ShouldAddRequirementVoidedHistoryRecord()
        {
            // Arrange
            Assert.IsNull(_historyAdded);

            // Act
            var sourceGuid = Guid.NewGuid();

            var tagRequirement = new Mock<TagRequirement>().Object;
            tagRequirement.RequirementDefinitionId = _requirementDefinitionId;

            _dut.Handle(new TagRequirementVoidedEvent(_plant, sourceGuid, tagRequirement), default);

            // Assert
            var expectedDescription = $"{EventType.RequirementVoided.GetDescription()} - '{_requirementDefinition.Title}'";

            Assert.IsNotNull(_historyAdded);
            Assert.AreEqual(_plant, _historyAdded.Plant);
            Assert.AreEqual(sourceGuid, _historyAdded.SourceGuid);
            Assert.IsNotNull(_historyAdded.Description);
            Assert.AreEqual(EventType.RequirementVoided, _historyAdded.EventType);
            Assert.AreEqual(ObjectType.Tag, _historyAdded.ObjectType);
            Assert.AreEqual(expectedDescription, _historyAdded.Description);
            Assert.IsFalse(_historyAdded.PreservationRecordGuid.HasValue);
            Assert.IsFalse(_historyAdded.DueInWeeks.HasValue);
        }
    }
}
