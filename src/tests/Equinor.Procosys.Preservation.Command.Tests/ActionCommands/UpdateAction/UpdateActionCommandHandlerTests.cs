using System;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.ActionCommands.UpdateAction;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Action = Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.Procosys.Preservation.Command.Tests.ActionCommands.UpdateAction
{
    [TestClass]
    public class UpdateActionCommandHandlerTests : CommandHandlerTestsBase
    {
        private readonly int _tagId = 2;
        private readonly int _actionId = 12;
        private readonly string _oldTitle = "ActionTitleOld";
        private readonly string _newTitle = "ActionTitleNew";
        private readonly string _oldDescription = "ActionDescriptionOld";
        private readonly string _newDescription = "ActionDescriptionNew";
        private readonly DateTime _oldDueTimeUtc = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc);
        private readonly DateTime _newDueTimeUtc = new DateTime(2020, 2, 2, 2, 1, 1, DateTimeKind.Utc);

        private UpdateActionCommand _command;
        private UpdateActionCommandHandler _dut;

        private Mock<IProjectRepository> _projectRepositoryMock;
        private Mock<Action> _actionMock;

        [TestInitialize]
        public void Setup()
        {
            _projectRepositoryMock = new Mock<IProjectRepository>();

            var tagMock = new Mock<Tag>();
            tagMock.SetupGet(t => t.Plant).Returns(TestPlant);
            tagMock.SetupGet(t => t.Id).Returns(_tagId);
            _actionMock = new Mock<Action>(TestPlant, _oldTitle, _oldDescription, _oldDueTimeUtc);
            _actionMock = new Mock<Action>();
            _actionMock.SetupGet(t => t.Plant).Returns(TestPlant);
            _actionMock.SetupGet(t => t.Id).Returns(_actionId);
            tagMock.Object.AddAction(_actionMock.Object);

            _projectRepositoryMock
                .Setup(r => r.GetTagByTagIdAsync(_tagId))
                .Returns(Task.FromResult(tagMock.Object));

            _command = new UpdateActionCommand(_tagId, _actionId, _newTitle, _newDescription, _newDueTimeUtc, null);

            _dut = new UpdateActionCommandHandler(
                _projectRepositoryMock.Object,
                UnitOfWorkMock.Object
            );
        }

        [TestMethod]
        public async Task HandlingUpdateActionCommand_ShouldUpdateAction()
        {
            var result = await _dut.Handle(_command, default);
            Assert.AreEqual(0, result.Errors.Count);

            Assert.AreEqual(_actionMock.Object.Title, _newTitle);
            Assert.AreEqual(_actionMock.Object.Description, _newDescription);
            Assert.AreEqual(_actionMock.Object.DueTimeUtc, _newDueTimeUtc);
        }

        [TestMethod]
        public async Task HandlingUpdateActionCommand_ShouldSetRowVersion()
        {
            await _dut.Handle(_command, default);

            _actionMock.Verify(u => u.SetRowVersion(_command.RowVersion), Times.Once);
        }

        public async Task HandlingUpdateActionCommand_ShouldSave()
        {
            await _dut.Handle(_command, default);
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}
