using System;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.ActionCommands.CloseAction;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Action = Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.Procosys.Preservation.Command.Tests.ActionCommands.CloseAction
{
    [TestClass]
    public class CloseActionCommandHandlerTests : CommandHandlerTestsBase
    {
        private readonly int _personId = 16;
        private readonly string _rowVersion = "AAAAAAAAABA=";

        private readonly  Guid _currentUserOid = new Guid("12345678-1234-1234-1234-123456789123");

        private CloseActionCommand _command;
        private CloseActionCommandHandler _dut;

        private Mock<IProjectRepository> _projectRepositoryMock;
        private Action _action;
        private Mock<IPersonRepository> _personRepositoryMock;
        private Mock<ICurrentUserProvider> _currentUserProviderMock;
        private Mock<Person> _personMock;

        [TestInitialize]
        public void Setup()
        {
            _projectRepositoryMock = new Mock<IProjectRepository>();
            _currentUserProviderMock = new Mock<ICurrentUserProvider>();
            _personRepositoryMock = new Mock<IPersonRepository>();

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

            _projectRepositoryMock
                .Setup(r => r.GetTagByTagIdAsync(tagId))
                .Returns(Task.FromResult(tagMock.Object));

            _currentUserProviderMock
                .Setup(x => x.GetCurrentUserOid())
                .Returns(_currentUserOid);

            _personRepositoryMock
                .Setup(p => p.GetByOidAsync(It.Is<Guid>(x => x == _currentUserOid)))
                .Returns(Task.FromResult(_personMock.Object));

            _command = new CloseActionCommand(tagId, actionId, _rowVersion, TestUserOid);

            _dut = new CloseActionCommandHandler(
                _projectRepositoryMock.Object,
                UnitOfWorkMock.Object,
                _personRepositoryMock.Object,
                _currentUserProviderMock.Object
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
        public async Task HandlingCloseActionCommand_ShouldSetAndReturnRowVersion()
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
        public async Task HandlingCloseActionCommand_ShouldSave()
        {
            await _dut.Handle(_command, default);
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(_command.CurrentUserOid, default), Times.Once);
        }
    }
}
