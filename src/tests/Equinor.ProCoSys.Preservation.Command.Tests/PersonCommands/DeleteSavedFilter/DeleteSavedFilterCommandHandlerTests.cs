using System.Threading.Tasks;
using Equinor.ProCoSys.Auth.Misc;
using Equinor.ProCoSys.Preservation.Command.PersonCommands.DeleteSavedFilter;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.PersonCommands.DeleteSavedFilter
{
    [TestClass]
    public class DeleteSavedFilterCommandHandlerTests : CommandHandlerTestsBase
    {
        private int _savedFilterId = 1;
        
        private Mock<IPersonRepository> _personRepositoryMock;
        private Mock<ICurrentUserProvider> _currentUserProviderMock;
        private DeleteSavedFilterCommand _command;
        private DeleteSavedFilterCommandHandler _dut;
        private Mock<SavedFilter> _savedFilterMock;
        private Person _person;

        [TestInitialize]
        public void Setup()
        {
            const string RowVersion = "AAAAAAAAABA=";

            // Arrange
            _person = new Person(CurrentUserOid, "firstName", "lastName");

            _personRepositoryMock = new Mock<IPersonRepository>();
            _personRepositoryMock.Setup(p => p.GetWithSavedFiltersByOidAsync(CurrentUserOid))
                .Returns(Task.FromResult(_person));
            _currentUserProviderMock = new Mock<ICurrentUserProvider>();
            _currentUserProviderMock
                .Setup(x => x.GetCurrentUserOid())
                .Returns(CurrentUserOid);

            _savedFilterMock = new Mock<SavedFilter>();
            _savedFilterMock.SetupGet(s => s.Id).Returns(_savedFilterId);

            _person.AddSavedFilter(_savedFilterMock.Object);
            _command = new DeleteSavedFilterCommand(_savedFilterId, RowVersion);

            _dut = new DeleteSavedFilterCommandHandler(_personRepositoryMock.Object, UnitOfWorkMock.Object,
                _currentUserProviderMock.Object);
        }

        [TestMethod]
        public async Task HandlingDeleteSavedFilterCommand_ShouldDeleteSavedFilterFromPerson()
        {
            // Arrange
            Assert.AreEqual(1, _person.SavedFilters.Count);
            // Act
            await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, _person.SavedFilters.Count);
            _personRepositoryMock.Verify(r => r.RemoveSavedFilter(_savedFilterMock.Object), Times.Once);
        }

        [TestMethod]
        public async Task HandlingDeleteSavedFilterCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);

            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}
