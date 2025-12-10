using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.TagCommands.DeleteTag;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.TagCommands.DeleteTag
{
    [TestClass]
    public class DeleteTagCommandHandlerTests : CommandHandlerTestsBase
    {
        private int _tagId = 2;
        private const string _projectName = "ProjectName";
        private const string _rowVersion = "AAAAAAAAABA=";
        private DeleteTagCommand _command;
        private DeleteTagCommandHandler _dut;
        private Tag _tag;
        private Project _project;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            var _stepMock = new Mock<Step>();
            _stepMock.SetupGet(s => s.Plant).Returns(TestPlant);

            var _rdMock = new Mock<RequirementDefinition>();
            _rdMock.SetupGet(rd => rd.Id).Returns(2);
            _rdMock.SetupGet(rd => rd.Plant).Returns(TestPlant);

            var requirement = new TagRequirement(TestPlant, 2, _rdMock.Object);
            _tag = new Tag(TestPlant, TagType.Standard, Guid.NewGuid(), "", "", _stepMock.Object, new List<TagRequirement> { requirement });
            _tag.SetProtectedIdForTesting(2);
            _tag.IsVoided = true;

            _project = new Project(TestPlant, _projectName, "", ProjectProCoSysGuid);
            _project.AddTag(_tag);

            var projectRepositoryMock = new Mock<IProjectRepository>();
            projectRepositoryMock
                .Setup(x => x.GetProjectAndTagWithPreservationHistoryByTagIdAsync(_tag.Id))
                .Returns(Task.FromResult(_project));
            _command = new DeleteTagCommand(_tagId, _rowVersion);

            _dut = new DeleteTagCommandHandler(projectRepositoryMock.Object, UnitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task HandlingDeleteTagCommand_ShouldDeleteTagFromScope()
        {
            // Arrange
            Assert.AreEqual(1, _project.Tags.Count);

            // Act
            await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, _project.Tags.Count);
        }

        [TestMethod]
        public async Task HandlingDeleteTagCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);

            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        [TestMethod]
        public async Task HandlingDeleteTagCommand_ShouldAddDeletionEvent()
        {
            // Act
            await _dut.Handle(_command, default);
            var eventTypes = _tag.DomainEvents.Select(e => e.GetType()).ToList();

            // Assert
            CollectionAssert.Contains(eventTypes, typeof(DeletedEvent<Tag>));
        }
    }
}
