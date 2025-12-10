using System;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Command.ActionCommands.UpdateAction;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Command.Tests.ActionCommands.UpdateAction
{
    [TestClass]
    public class UpdateActionCommandHandlerTests : CommandHandlerTestsBase
    {
        private readonly string _oldTitle = "ActionTitleOld";
        private readonly string _newTitle = "ActionTitleNew";
        private readonly string _oldDescription = "ActionDescriptionOld";
        private readonly string _newDescription = "ActionDescriptionNew";
        private readonly DateTime _oldDueTimeUtc = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc);
        private readonly DateTime _newDueTimeUtc = new DateTime(2020, 2, 2, 2, 1, 1, DateTimeKind.Utc);
        private readonly string _rowVersion = "AAAAAAAAABA=";

        private UpdateActionCommand _command;
        private UpdateActionCommandHandler _dut;

        private Action _action;

        [TestInitialize]
        public void Setup()
        {
            var tagId = 2;
            var tagMock = new Mock<Tag>();
            tagMock.SetupGet(t => t.Plant).Returns(TestPlant);
            tagMock.SetupGet(t => t.Id).Returns(tagId);
            var actionId = 12;
            _action = new Action(TestPlant, _oldTitle, _oldDescription, _oldDueTimeUtc);
            _action.SetProtectedIdForTesting(actionId);
            tagMock.Object.AddAction(_action);

            var projectRepositoryMock = new Mock<IProjectRepository>();
            projectRepositoryMock
                .Setup(r => r.GetTagWithActionsByTagIdAsync(tagId))
                .Returns(Task.FromResult(tagMock.Object));

            _command = new UpdateActionCommand(tagId, actionId, _newTitle, _newDescription, _newDueTimeUtc, _rowVersion);

            _dut = new UpdateActionCommandHandler(
                projectRepositoryMock.Object,
                UnitOfWorkMock.Object
            );
        }

        [TestMethod]
        public async Task HandlingUpdateActionCommand_ShouldUpdateAction()
        {
            // Arrange
            Assert.AreEqual(_oldTitle, _action.Title);
            Assert.AreEqual(_oldDescription, _action.Description);
            Assert.AreEqual(_oldDueTimeUtc, _action.DueTimeUtc);

            // Act
            await _dut.Handle(_command, default);

            // Arrange
            Assert.AreEqual(_newTitle, _action.Title);
            Assert.AreEqual(_newDescription, _action.Description);
            Assert.AreEqual(_newDueTimeUtc, _action.DueTimeUtc);
        }

        [TestMethod]
        public async Task HandlingUpdateActionCommand_ShouldSetAndReturnRowVersion()
        {
            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            // In real life EF Core will create a new RowVersion when save.
            // Since UnitOfWorkMock is a Mock this will not happen here, so we assert that RowVersion is set from command
            Assert.AreEqual(_rowVersion, result.Data);
            Assert.AreEqual(_rowVersion, _action.RowVersion.ConvertToString());
        }

        [TestMethod]
        public async Task HandlingUpdateActionCommand_ShouldSave()
        {
            await _dut.Handle(_command, default);
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}
