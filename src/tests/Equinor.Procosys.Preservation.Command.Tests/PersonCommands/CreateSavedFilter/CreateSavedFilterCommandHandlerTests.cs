using System;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.PersonCommands.CreateSavedFilter;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.PersonCommands.CreateSavedFilter
{
    [TestClass]
    public class CreateSavedFilterCommandHandlerTests : CommandHandlerTestsBase
    {
        private Mock<IPersonRepository> _personRepositoryMock;
        private Mock<ICurrentUserProvider> _currentUserProviderMock;
        private Mock<IProjectRepository> _projectRepositoryMock;
        private Person _person;
        private Project _project;
        private CreateSavedFilterCommand _command;
        private CreateSavedFilterCommandHandler _dut;

        private const string _title = "T1";
        private const string _criteria = "C1";
        private const string _projectName = "Project";
        private int _projectId = 1;
        private readonly Guid _currentUserOid = new Guid("12345678-1234-1234-1234-123456789123");

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            
            _project = new Project(TestPlant, _projectName, "desc");
            _project.SetProtectedIdForTesting(_projectId);
            _projectRepositoryMock = new Mock<IProjectRepository>();
            _projectRepositoryMock.Setup(p => p.GetProjectOnlyByNameAsync(_projectName))
                .Returns(Task.FromResult(_project));

            _person = new Person(_currentUserOid, "Current", "User");
            _personRepositoryMock = new Mock<IPersonRepository>();
            _personRepositoryMock
                .Setup(p => p.GetWithSavedFiltersByOidAsync(It.Is<Guid>(x => x == CurrentUserOid)))
                .Returns(Task.FromResult(_person));

            _currentUserProviderMock = new Mock<ICurrentUserProvider>();
            _currentUserProviderMock
                .Setup(x => x.GetCurrentUserOid())
                .Returns(CurrentUserOid);

            _command = new CreateSavedFilterCommand(_projectName, _title, _criteria, true);

            _dut = new CreateSavedFilterCommandHandler(
                _personRepositoryMock.Object,
                UnitOfWorkMock.Object,
                PlantProviderMock.Object,
                _currentUserProviderMock.Object,
                _projectRepositoryMock.Object);
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
            Assert.IsTrue(savedFilter.DefaultFilter);
            Assert.AreEqual(_projectId, savedFilter.ProjectId);
            Assert.AreEqual(_title, savedFilter.Title);
            Assert.AreEqual(_criteria, savedFilter.Criteria);
        }

        [TestMethod]
        public async Task HandlingCreateSavedFilterCommand_ShouldOverrideDefaultValue()
        {
            await _dut.Handle(_command, default);
            Assert.AreEqual(1, _person.SavedFilters.Count);

            // Act
            _command = new CreateSavedFilterCommand(_projectName, "T2", "C2", true);
            var result = await _dut.Handle(_command, default);
            var savedFilter = _person.SavedFilters.First();
            var addedSavedFilter = _person.SavedFilters.Last();

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(0, result.Data);
            Assert.IsFalse(savedFilter.DefaultFilter);
            Assert.IsTrue(addedSavedFilter.DefaultFilter);
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
