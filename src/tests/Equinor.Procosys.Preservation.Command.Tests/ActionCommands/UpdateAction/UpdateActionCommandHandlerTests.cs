using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.ActionCommands.UpdateAction;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Action = Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;
using TagRequirement = Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.Requirement;

namespace Equinor.Procosys.Preservation.Command.Tests.ActionCommands.UpdateAction
{
    [TestClass]
    public class UpdateActionCommandHandlerTests : CommandHandlerTestsBase
    {
        private readonly int _tagId = 2;
        private readonly int _actionId = 0;
        private readonly string _oldTitle = "ActionTitleOld";
        private readonly string _newTitle = "ActionTitleNew";
        private readonly string _oldDescription = "ActionDescriptionOld";
        private readonly string _newDescription = "ActionDescriptionNew";
        private readonly DateTime _oldDueTimeUtc = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc);
        private readonly DateTime _newDueTimeUtc = new DateTime(2020, 2, 2, 2, 1, 1, DateTimeKind.Utc);

        private TagRequirement _requirement;
        private Tag _tag;
        private UpdateActionCommand _command;
        private UpdateActionCommandHandler _dut;

        private Mock<IProjectRepository> _projectRepositoryMock;
        private readonly int _rdId1 = 17;
        private readonly int _intervalWeeks = 2;

        [TestInitialize]
        public void Setup()
        {
            _projectRepositoryMock = new Mock<IProjectRepository>();

            var stepMock = new Mock<Step>();
            stepMock.SetupGet(s => s.Plant).Returns(TestPlant);

            var _rdMock = new Mock<RequirementDefinition>();
            _rdMock.SetupGet(rd => rd.Id).Returns(_rdId1);
            _rdMock.SetupGet(rd => rd.Plant).Returns(TestPlant);

            _requirement = new TagRequirement(TestPlant, _intervalWeeks, _rdMock.Object);
            _tag = new Tag(TestPlant, TagType.Standard, "", "", stepMock.Object, new List<TagRequirement> { _requirement });
            var action = new Action(TestPlant, _oldTitle, _oldDescription, _oldDueTimeUtc);
            _tag.AddAction(action);

            _projectRepositoryMock
                .Setup(r => r.GetTagByTagIdAsync(_tagId))
                .Returns(Task.FromResult(_tag));

            _command = new UpdateActionCommand(_tagId, _actionId, _newTitle, _newDescription, _newDueTimeUtc);

            _dut = new UpdateActionCommandHandler(
                _projectRepositoryMock.Object,
                UnitOfWorkMock.Object
            );
        }

        [TestMethod]
        public async Task HandlingUpdateActionCommand_ShouldUpdateAction()
        {
            Assert.IsTrue(_tag.Actions.Count == 1);

            var result = await _dut.Handle(_command, default);
            var action = new List<Action>(_tag.Actions)[0];
            Assert.AreEqual(0, result.Errors.Count);
            Assert.IsTrue(_tag.Actions.Count == 1);
            Assert.AreEqual(action.Title, _newTitle);
            Assert.AreEqual(action.Description, _newDescription);
            Assert.AreEqual(action.DueTimeUtc, _newDueTimeUtc);
        }

        [TestMethod]
        public async Task HandlingUpdateActionCommand_ShouldSave()
        {
            await _dut.Handle(_command, default);
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        [TestMethod]
        public async Task HandlingUpdateActionCommand_ShouldThrowException_WhenDueIsNotUtc()
        {
            var command = new UpdateActionCommand(
                _tagId,
                _actionId,
                _newTitle,
                _newDescription,
                new DateTime(2020, 1, 1, 1, 1, 1));

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
            _dut.Handle(command, default)
            );
        }
    }
}
