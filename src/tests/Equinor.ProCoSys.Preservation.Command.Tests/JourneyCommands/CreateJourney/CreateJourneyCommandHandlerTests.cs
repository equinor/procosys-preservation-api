using System;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.JourneyCommands.CreateJourney;
using Equinor.ProCoSys.Preservation.Command.Services.ProjectImportService;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.JourneyCommands.CreateJourney
{
    [TestClass]
    public class CreateJourneyCommandHandlerTests : CommandHandlerTestsBase
    {
        private const string TestJourney = "TestJourney";
        private Journey _journeyAdded;
        private Mock<IJourneyRepository> _journeyRepositoryMock;
        private CreateJourneyCommand _command;
        private CreateJourneyCommandHandler _dut;
        private Mock<IProjectImportService> _projectImportService;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            _journeyRepositoryMock = new Mock<IJourneyRepository>();
            _journeyRepositoryMock
                .Setup(repo => repo.Add(It.IsAny<Journey>()))
                .Callback<Journey>(journey =>
                {
                    _journeyAdded = journey;
                });

            _projectImportService = new Mock<IProjectImportService>();

            _command = new CreateJourneyCommand(TestJourney);

            _dut = new CreateJourneyCommandHandler(
                _journeyRepositoryMock.Object,
                UnitOfWorkMock.Object,
                PlantProviderMock.Object, _projectImportService.Object);
        }

        [TestMethod]
        public async Task HandlingCreateJourneyCommand_ShouldAddJourneyToRepository()
        {
            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(0, result.Data);
            Assert.AreEqual(0, _journeyAdded.Id);
            Assert.AreEqual(TestJourney, _journeyAdded.Title);
            Assert.AreEqual(TestPlant, _journeyAdded.Plant);
        }

        [TestMethod]
        public async Task HandlingCreateJourneyCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);

            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        [TestMethod]
        public async Task HandlingCreateJourneyCommand_ShouldNotGetProject_WhenNoProject()
        {
            // Arrange
            var commandWithoutProject = new CreateJourneyCommand(TestJourney);
            // Act
            await _dut.Handle(commandWithoutProject, default);

            // Assert
            Assert.AreEqual(TestJourney, _journeyAdded.Title);
            Assert.AreEqual(TestPlant, _journeyAdded.Plant);
            _projectImportService.Verify(service => service.TryGetOrImportProjectAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [TestMethod]
        public async Task HandlingCreateJourneyCommand_ShouldGetProject_WhenProjectIsAssigned()
        {
            // Arrange
            var projectName = "ExistingProject";
            var existingProject = new Project(TestPlant, projectName, "Description", Guid.NewGuid());
            _projectImportService.Setup(repo => repo.TryGetOrImportProjectAsync(projectName, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingProject);
            var commandWithProject = new CreateJourneyCommand(TestJourney, projectName);

            // Act
            await _dut.Handle(commandWithProject, default);

            // Assert
            Assert.AreEqual(TestJourney, _journeyAdded.Title);
            Assert.AreEqual(TestPlant, _journeyAdded.Plant);
            Assert.AreEqual(existingProject, _journeyAdded.Project);
            _projectImportService.Verify(service => service.TryGetOrImportProjectAsync(projectName, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
