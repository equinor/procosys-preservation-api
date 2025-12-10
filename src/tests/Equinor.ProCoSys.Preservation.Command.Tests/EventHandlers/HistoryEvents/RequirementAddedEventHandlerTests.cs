
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
    public class RequirementAddedEventHandlerTests
    {
        private const int RequirementDefinitionId = 3;
        private const string Plant = "TestPlant";

        private Mock<IRequirementTypeRepository> _requirementTypeRepositoryMock;
        private Mock<IHistoryRepository> _historyRepositoryMock;
        private TagRequirementAddedEventHandler _dut;
        private History _historyAdded;
        private RequirementDefinition _requirementDefinition;
        private Guid _tagGuid;
        private Mock<IProjectRepository> _projectRepository;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            _tagGuid = Guid.NewGuid();

            var mockTag = new Mock<Tag>().Object;
            mockTag.Guid = _tagGuid;

            _requirementDefinition = new RequirementDefinition(Plant, "Rotate 2 turns", 2, RequirementUsage.ForAll, 1);

            _historyRepositoryMock = new Mock<IHistoryRepository>();
            _historyRepositoryMock
                .Setup(repo => repo.Add(It.IsAny<History>()))
                .Callback<History>(history =>
                {
                    _historyAdded = history;
                });

            _requirementTypeRepositoryMock = new Mock<IRequirementTypeRepository>();
            _requirementTypeRepositoryMock
                .Setup(repo => repo.GetRequirementDefinitionByIdAsync(RequirementDefinitionId))
                .Returns(Task.FromResult(_requirementDefinition));

            _projectRepository = new Mock<IProjectRepository>();
            _projectRepository
                .Setup(p => p.GetTagOnlyByTagIdAsync(It.IsAny<int>()))
                .ReturnsAsync(mockTag);

            _projectRepository
                .Setup(p => p.GetTagByTagRequirementGuidAsync(It.IsAny<Guid>()))
                .ReturnsAsync(mockTag);

            _dut = new TagRequirementAddedEventHandler(_historyRepositoryMock.Object, _requirementTypeRepositoryMock.Object, _projectRepository.Object);
        }

        [TestMethod]
        public void Handle_ShouldAddRequirementAddedHistoryRecord()
        {
            // Arrange
            Assert.IsNull(_historyAdded);

            // Act
            var tagRequirement = new Mock<TagRequirement>().Object;
            tagRequirement.RequirementDefinitionId = RequirementDefinitionId;

            _dut.Handle(new TagRequirementAddedEvent(Plant, _tagGuid, tagRequirement), default);

            // Assert
            var expectedDescription = $"{EventType.RequirementAdded.GetDescription()} - '{_requirementDefinition.Title}'";

            Assert.IsNotNull(_historyAdded);
            Assert.AreEqual(Plant, _historyAdded.Plant);
            Assert.AreEqual(_tagGuid, _historyAdded.SourceGuid);
            Assert.IsNotNull(_historyAdded.Description);
            Assert.AreEqual(EventType.RequirementAdded, _historyAdded.EventType);
            Assert.AreEqual(ObjectType.Tag, _historyAdded.ObjectType);
            Assert.AreEqual(expectedDescription, _historyAdded.Description);
            Assert.IsFalse(_historyAdded.PreservationRecordGuid.HasValue);
            Assert.IsFalse(_historyAdded.DueInWeeks.HasValue);
        }
    }
}
