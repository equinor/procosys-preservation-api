using System;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.ActionCommands.CloseAction;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Action = Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.Procosys.Preservation.Command.Tests.ActionCommands.CloseAction
{
    [TestClass]
    public class CloseActionCommandHandlerTests : CommandHandlerTestsBase
    {
        private readonly int _tagId = 2;
        private readonly int _actionId = 12;
        private readonly int _personId = 16;

        private readonly  Guid _currentUserOid = new Guid("12345678-1234-1234-1234-123456789123");

        private CloseActionCommand _command;
        private CloseActionCommandHandler _dut;

        private Mock<IProjectRepository> _projectRepositoryMock;
        private Mock<Action> _actionMock;
        private Mock<IPersonRepository> _personRepositoryMock;
        private Mock<ICurrentUserProvider> _currentUserProviderMock;
        private Mock<Person> _personMock;

        [TestInitialize]
        public void Setup()
        {
            _projectRepositoryMock = new Mock<IProjectRepository>();
            _currentUserProviderMock = new Mock<ICurrentUserProvider>();
            _personRepositoryMock = new Mock<IPersonRepository>();

            var tagMock = new Mock<Tag>();
            tagMock.SetupGet(t => t.Plant).Returns(TestPlant);
            tagMock.SetupGet(t => t.Id).Returns(_tagId);
            _actionMock = new Mock<Action>();
            _actionMock.SetupGet(t => t.Plant).Returns(TestPlant);
            _actionMock.SetupGet(t => t.Id).Returns(_actionId);
            tagMock.Object.AddAction(_actionMock.Object);

            _personMock = new Mock<Person>();
            _personMock.SetupGet(p => p.Id).Returns(_personId);

            _projectRepositoryMock
                .Setup(r => r.GetTagByTagIdAsync(_tagId))
                .Returns(Task.FromResult(tagMock.Object));

            _currentUserProviderMock
                .Setup(x => x.GetCurrentUserOid())
                .Returns(_currentUserOid);

            _personRepositoryMock
                .Setup(p => p.GetByOidAsync(It.Is<Guid>(x => x == _currentUserOid)))
                .Returns(Task.FromResult(_personMock.Object));

            _command = new CloseActionCommand(_tagId, _actionId, null);

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
            var result = await _dut.Handle(_command, default);
            Assert.AreEqual(0, result.Errors.Count);

            Assert.IsTrue(_actionMock.Object.IsClosed);
        }

        [TestMethod]
        public async Task HandlingCloseActionCommand_ShouldCloseByPerson()
        {
            var result = await _dut.Handle(_command, default);
            Assert.AreEqual(_personId, _actionMock.Object.ClosedById);
        }

        [TestMethod]
        public async Task HandlingCloseActionCommand_ShouldSetRowVersion()
        {
            await _dut.Handle(_command, default);

            _actionMock.Verify(u => u.SetRowVersion(_command.RowVersion), Times.Once);
        }

        public async Task HandlingCloseActionCommand_ShouldSave()
        {
            await _dut.Handle(_command, default);
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}
