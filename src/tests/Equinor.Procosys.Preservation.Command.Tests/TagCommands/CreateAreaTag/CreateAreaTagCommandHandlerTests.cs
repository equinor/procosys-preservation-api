using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.TagCommands.CreateAreaTag;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.MainApi.Area;
using Equinor.Procosys.Preservation.MainApi.Discipline;
using Equinor.Procosys.Preservation.MainApi.Project;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Requirement = Equinor.Procosys.Preservation.Command.TagCommands.Requirement;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.CreateAreaTag
{
    [TestClass]
    public class CreateAreaTagCommandHandlerTests : CommandHandlerTestsBase
    {
        private const string TestProjectName = "TestProjectX";
        private const int StepId = 11;
        private const int ReqDefId1 = 99;
        private const int ReqDefId2 = 199;
        private const int Interval1 = 2;
        private const int Interval2 = 3;

        private Mock<Step> _stepMock;
        private Mock<IJourneyRepository> _journeyRepositoryMock;
        private Project _projectAddedToRepository;
        private Mock<IProjectRepository> _projectRepositoryMock;
        private Mock<IRequirementTypeRepository> _rtRepositoryMock;
        private Mock<IProjectApiService> _projectApiServiceMock;
        private Mock<IDisciplineApiService> _disciplineApiServiceMock;
        private Mock<IAreaApiService> _areaApiServiceMock;

        private CreateAreaTagCommand _command;
        private CreateAreaTagCommandHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            _stepMock = new Mock<Step>();
            _stepMock.SetupGet(s => s.Id).Returns(StepId);
            _stepMock.SetupGet(s => s.Plant).Returns(TestPlant);
            
            _journeyRepositoryMock = new Mock<IJourneyRepository>();
            _journeyRepositoryMock
                .Setup(x => x.GetStepByStepIdAsync(StepId))
                .Returns(Task.FromResult(_stepMock.Object));

            _projectRepositoryMock = new Mock<IProjectRepository>();
            _projectRepositoryMock
                .Setup(x => x.Add(It.IsAny<Project>()))
                .Callback<Project>(project =>
                {
                    _projectAddedToRepository = project;
                });

            _rtRepositoryMock = new Mock<IRequirementTypeRepository>();
            var rdMock1 = new Mock<RequirementDefinition>();
            rdMock1.SetupGet(x => x.Id).Returns(ReqDefId1);
            rdMock1.SetupGet(x => x.Plant).Returns(TestPlant);
            var rdMock2 = new Mock<RequirementDefinition>();
            rdMock2.SetupGet(x => x.Id).Returns(ReqDefId2);
            rdMock2.SetupGet(x => x.Plant).Returns(TestPlant);
            _rtRepositoryMock
                .Setup(r => r.GetRequirementDefinitionsByIdsAsync(new List<int> {ReqDefId1, ReqDefId2}))
                .Returns(Task.FromResult(new List<RequirementDefinition> {rdMock1.Object, rdMock2.Object}));

            var disciplineCode = "D";
            var areaCode = "A";

            _projectApiServiceMock = new Mock<IProjectApiService>();
            _projectApiServiceMock.Setup(s => s.GetProject(TestPlant, TestProjectName))
                .Returns(Task.FromResult(new ProcosysProject {Description = "ProjectDescription"}));

            _disciplineApiServiceMock = new Mock<IDisciplineApiService>();
            _disciplineApiServiceMock.Setup(s => s.GetDisciplines(TestPlant))
                .Returns(Task.FromResult(new List<ProcosysDiscipline>
                {
                    new ProcosysDiscipline {Code = disciplineCode, Description = "DisciplineDescription"}
                }));

            _areaApiServiceMock = new Mock<IAreaApiService>();
            _areaApiServiceMock.Setup(s => s.GetAreas(TestPlant))
                .Returns(Task.FromResult(new List<ProcosysArea>
                {
                    new ProcosysArea {Code = areaCode, Description = "AreaDescription"}
                }));

            _command = new CreateAreaTagCommand(
                TestProjectName,
                TagType.PreArea,
                disciplineCode,
                areaCode,
                null,
                StepId,
                new List<Requirement>
                {
                    new Requirement(ReqDefId1, Interval1),
                    new Requirement(ReqDefId2, Interval2),
                },
                null,
                "Remark",
                "SA");

            _dut = new CreateAreaTagCommandHandler(
                _projectRepositoryMock.Object,
                _journeyRepositoryMock.Object,
                _rtRepositoryMock.Object,
                UnitOfWorkMock.Object,
                PlantProviderMock.Object,
                _projectApiServiceMock.Object,
                _disciplineApiServiceMock.Object,
                _areaApiServiceMock.Object);
        }

        [TestMethod]
        public async Task HandlingCreateAreaTagCommand_ShouldAddProjectToRepository_WhenProjectNotExists()
        {
            // Arrange
            _projectRepositoryMock
                .Setup(r => r.GetByNameAsync(TestProjectName)).Returns(Task.FromResult((Project)null));

            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(0, _projectAddedToRepository.Id);
            Assert.AreEqual(TestProjectName, _projectAddedToRepository.Name);
            Assert.AreEqual("ProjectDescription", _projectAddedToRepository.Description);
        }

        [TestMethod]
        public async Task HandlingCreateAreaTagCommand_ShouldNotAddAnyProjectToRepository_WhenProjectAlreadyExists()
        {
            // Arrange
            var project = new Project(TestPlant, TestProjectName, "");
            _projectRepositoryMock
                .Setup(r => r.GetByNameAsync(TestProjectName)).Returns(Task.FromResult(project));

            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.IsNull(_projectAddedToRepository);
        }

        [TestMethod]
        public async Task HandlingCreateAreaTagCommand_ShouldAddTagToNewProject_WhenProjectNotExists()
        {
            // Arrange
            _projectRepositoryMock
                .Setup(r => r.GetByNameAsync(TestProjectName)).Returns(Task.FromResult((Project)null));

            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            
            var tags = _projectAddedToRepository.Tags;
            Assert.AreEqual(1, tags.Count);
            AssertTagProperties(_command, tags.First());
        }

        [TestMethod]
        public async Task HandlingCreateAreaTagCommand_ShouldAddTagToExistingProject_WhenProjectAlreadyExists()
        {
            // Arrange
            var project = new Project(TestPlant, TestProjectName, "");
            _projectRepositoryMock
                .Setup(r => r.GetByNameAsync(TestProjectName)).Returns(Task.FromResult(project));
            Assert.AreEqual(0, project.Tags.Count);
            
            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            
            var tags = project.Tags;
            Assert.AreEqual(1, tags.Count);
            AssertTagProperties(_command, tags.First());
        }

        [TestMethod]
        public async Task HandlingCreateAreaTagCommand_ShouldSave()
        {
            _projectRepositoryMock
                .Setup(r => r.GetByNameAsync(TestProjectName)).Returns(Task.FromResult(new Project(TestPlant, TestProjectName, "")));
            // Act
            await _dut.Handle(_command, default);
            
            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        private void AssertTagProperties(CreateAreaTagCommand command, Tag tagAddedToProject)
        {
            Assert.AreEqual(command.AreaCode, tagAddedToProject.AreaCode);
            Assert.IsNull(tagAddedToProject.Calloff);
            Assert.IsNull(tagAddedToProject.CommPkgNo);
            Assert.AreEqual(command.DisciplineCode, tagAddedToProject.DisciplineCode);
            Assert.AreEqual(0, tagAddedToProject.Id);
            Assert.AreEqual(command.TagType, tagAddedToProject.TagType);
            Assert.IsNull(tagAddedToProject.McPkgNo);
            Assert.AreEqual(command.Description, tagAddedToProject.Description);
            Assert.AreEqual(command.Remark, tagAddedToProject.Remark);
            Assert.AreEqual(command.StorageArea, tagAddedToProject.StorageArea);
            Assert.IsNull(tagAddedToProject.PurchaseOrderNo);
            Assert.AreEqual(TestPlant, tagAddedToProject.Plant);
            Assert.AreEqual(StepId, tagAddedToProject.StepId);
            Assert.IsNull(tagAddedToProject.TagFunctionCode);
            Assert.AreEqual(command.GetTagNo(), tagAddedToProject.TagNo);
            Assert.AreEqual(2, tagAddedToProject.Requirements.Count);
            AssertReqProperties(tagAddedToProject.Requirements.First(), ReqDefId1, Interval1);
            AssertReqProperties(tagAddedToProject.Requirements.Last(), ReqDefId2, Interval2);
        }

        private void AssertReqProperties(Domain.AggregateModels.ProjectAggregate.Requirement req, int reqDefId, int interval)
        {
            Assert.AreEqual(reqDefId, req.RequirementDefinitionId);
            Assert.AreEqual(interval, req.IntervalWeeks);
        }
    }
}
