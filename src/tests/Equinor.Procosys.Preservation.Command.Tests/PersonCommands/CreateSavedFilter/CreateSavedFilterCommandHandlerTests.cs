using System;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.ActionCommands.CreateAction;
using Equinor.Procosys.Preservation.Command.PersonCommands.CreateSavedFilter;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Infrastructure.Migrations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.PersonCommands.CreateSavedFilter
{
    [TestClass]
    public class CreateSavedFilterCommandHandlerTests : CommandHandlerTestsBase
    {
        private Mock<IPersonRepository> _personRepositoryMock;
        private Mock<ICurrentUserProvider> _currentUserProviderMock;
        private Person _person;
        private CreateSavedFilterCommand _command;
        private CreateSavedFilterCommandHandler _dut;

        private const string title = "T1";
        private const string criteria = "C1";
        private readonly Guid _currentUserOid = new Guid("12345678-1234-1234-1234-123456789123");

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            _person = new Person(_currentUserOid, "Current", "User");

            _personRepositoryMock = new Mock<IPersonRepository>();
            _personRepositoryMock
                .Setup(p => p.GetByOidAsync(It.Is<Guid>(x => x == CurrentUserOid)))
                .Returns(Task.FromResult(_person));

            _currentUserProviderMock = new Mock<ICurrentUserProvider>();
            _currentUserProviderMock
                .Setup(x => x.GetCurrentUserOid())
                .Returns(CurrentUserOid);

            _command = new CreateSavedFilterCommand(title, criteria, true);

            _dut = new CreateSavedFilterCommandHandler(
                _personRepositoryMock.Object,
                UnitOfWorkMock.Object,
                PlantProviderMock.Object,
                _currentUserProviderMock.Object);
        }

        [TestMethod]
        public async Task HandlingCreateSavedFilterCommand_ShouldAddSavedFilterToPerson()
        {
            // Act
            var result = await _dut.Handle(_command, default);
            var savedFilter = _person.SavedFilters.First();

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(0, result.Data);
            Assert.AreEqual(true, savedFilter.DefaultFilter);
            Assert.AreEqual(title, savedFilter.Title);
            Assert.AreEqual(criteria, savedFilter.Criteria);
        }

        [TestMethod]
        public async Task HandlingCreateSavedFilterCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);

            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}
