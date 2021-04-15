using System;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.PersonCommands.UpdateSavedFilter;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.PersonCommands.UpdateSavedFilter
{
    [TestClass]
    public class UpdateSavedFilterCommandHandlerTests : CommandHandlerTestsBase
    {
        private readonly string _newTitle = "NewSavedFilterTitle";
        private readonly string _oldTitle = "OldSavedFilterTitle";
        private readonly string _newCriteria = "NewSavedFilterCriteria";
        private readonly string _oldCriteria = "OldSavedFilterCriteria";
        private bool _newDefaultFilter = true;
        private readonly string _rowVersion = "AAAAAAAAABA=";
        private readonly Guid _currentUserOid = new Guid();

        private UpdateSavedFilterCommand _command;
        private UpdateSavedFilterCommandHandler _dut;

        private Mock<ICurrentUserProvider> _currentUserProviderMock;
        private Mock<IPersonRepository> _personRepositoryMock;
        private Project _project;
        private Person _person;
        private SavedFilter _savedFilter;

        [TestInitialize]
        public void Setup()
        {
            _person = new Person(_currentUserOid, "Current", "User");
            _project = new Project(TestPlant, "T", "D");

            _currentUserProviderMock = new Mock<ICurrentUserProvider>();
            _currentUserProviderMock.Setup(x => x.GetCurrentUserOid())
                .Returns(_currentUserOid);

            _personRepositoryMock = new Mock<IPersonRepository>();
            _personRepositoryMock.Setup(x => x.GetWithSavedFiltersByOidAsync(_currentUserOid))
                .Returns(Task.FromResult(_person));

            _savedFilter = new SavedFilter(TestPlant, _project, _oldTitle, _oldCriteria);
            _savedFilter.SetProtectedIdForTesting(2);
            _person.AddSavedFilter(_savedFilter);

            _command = new UpdateSavedFilterCommand(_savedFilter.Id, _newTitle, _newCriteria, _newDefaultFilter, _rowVersion);

            _dut = new UpdateSavedFilterCommandHandler(
                UnitOfWorkMock.Object,
                _currentUserProviderMock.Object,
                _personRepositoryMock.Object
            );
        }

        [TestMethod]
        public async Task HandlingUpdateSavedFilterCommand_ShouldUpdateSavedFilter()
        {
            // Arrange
            Assert.AreEqual(_oldTitle, _savedFilter.Title);
            Assert.AreEqual(_oldCriteria, _savedFilter.Criteria);
            Assert.AreEqual(false, _savedFilter.DefaultFilter);

            // Act
            await _dut.Handle(_command, default);

            // Arrange
            Assert.AreEqual(_newTitle, _savedFilter.Title);
            Assert.AreEqual(_newCriteria, _savedFilter.Criteria);
            Assert.AreEqual(_newDefaultFilter, _savedFilter.DefaultFilter);
        }

        [TestMethod]
        public async Task HandlingUpdateSavedFilterCommand_ShouldNotUpdateDefaultFilter_IfDefaultFilterIsNull()
        {
            // Arrange
            _command = new UpdateSavedFilterCommand(_savedFilter.Id, _newTitle, _newCriteria, null, _rowVersion);

            // Act
            await _dut.Handle(_command, default);

            // Arrange
            Assert.AreEqual(_newTitle, _savedFilter.Title);
            Assert.AreEqual(_newCriteria, _savedFilter.Criteria);
            Assert.AreEqual(false, _savedFilter.DefaultFilter);
        }

        [TestMethod]
        public async Task HandlingUpdateSavedFilterCommand_ShouldSetAndReturnRowVersion()
        {
            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            // In real life EF Core will create a new RowVersion when save.
            // Since UnitOfWorkMock is a Mock this will not happen here, so we assert that RowVersion is set from command
            Assert.AreEqual(_rowVersion, result.Data);
            Assert.AreEqual(_rowVersion, _savedFilter.RowVersion.ConvertToString());
        }

        [TestMethod]
        public async Task HandlingUpdateSavedFilterCommand_ShouldSave()
        {
            await _dut.Handle(_command, default);
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}
