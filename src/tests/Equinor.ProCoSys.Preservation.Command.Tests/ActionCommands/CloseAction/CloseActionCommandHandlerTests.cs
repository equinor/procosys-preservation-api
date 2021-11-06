using System;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.ActionCommands.CloseAction;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Command.Tests.ActionCommands.CloseAction
{
    [TestClass]
    public class CloseActionCommandHandlerTests : CommandHandlerTestsBase
    {
        private readonly int _personId = 16;
        private readonly string _rowVersion = "AAAAAAAAABA=";

        private CloseActionCommand _command;
        private CloseActionCommandHandler _dut;

        private Action _action;
        private Mock<Person> _personMock;

        [TestInitialize]
        public void Setup()
        {
            var tagId = 2;
            var tagMock = new Mock<Tag>();
            tagMock.SetupGet(t => t.Plant).Returns(TestPlant);
            tagMock.SetupGet(t => t.Id).Returns(tagId);
            _action = new Action(TestPlant, "T", "D", null);
            var actionId = 12;
            _action.SetProtectedIdForTesting(actionId);
            tagMock.Object.AddAction(_action);

            _personMock = new Mock<Person>();
            _personMock.SetupGet(p => p.Id).Returns(_personId);

            var projectRepositoryMock = new Mock<IProjectRepository>();
            projectRepositoryMock
                .Setup(r => r.GetTagWithActionsByTagIdAsync(tagId))
                .Returns(Task.FromResult(tagMock.Object));

            var personRepositoryMock = new Mock<IPersonRepository>();
            personRepositoryMock
                .Setup(p => p.GetByOidAsync(It.Is<Guid>(x => x == CurrentUserOid)))
                .Returns(Task.FromResult(_personMock.Object));

            _command = new CloseActionCommand(tagId, actionId, _rowVersion);

            _dut = new CloseActionCommandHandler(
                projectRepositoryMock.Object,
                UnitOfWorkMock.Object,
                personRepositoryMock.Object,
                CurrentUserProviderMock.Object
            );
        }

        [TestMethod]
        public async Task HandlingCloseActionCommand_ShouldCloseAction()
        {
            await _dut.Handle(_command, default);

            Assert.IsTrue(_action.IsClosed);
        }

        [TestMethod]
        public async Task HandlingCloseActionCommand_ShouldCloseByPerson()
        {
            await _dut.Handle(_command, default);
            Assert.AreEqual(_personId, _action.ClosedById);
        }
                
        [TestMethod]
        public async Task HandlingCloseActionCommand_ShouldSetRowVersion()
        {
            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            // In real life EF Core will create a new RowVersion when save.
            // Since UnitOfWorkMock is a Mock this will not happen here, so we assert that RowVersion is set from command
            Assert.AreEqual(_rowVersion, _action.RowVersion.ConvertToString());
        }

        [TestMethod]
        public async Task HandlingCloseActionCommand_ShouldSave()
        {
            await _dut.Handle(_command, default);
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}
