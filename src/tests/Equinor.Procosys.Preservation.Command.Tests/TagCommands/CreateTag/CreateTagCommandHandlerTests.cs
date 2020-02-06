using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.TagCommands.CreateTag;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.MainApi.Tag;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Requirement = Equinor.Procosys.Preservation.Command.TagCommands.CreateTag.Requirement;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.CreateTag
{
    [TestClass]
    public class CreateTagCommandHandlerTests : CommandHandlerTestsBase
    {
        private const string TestTagNo1 = "TagNo1";
        private const string TestTagNo2 = "TagNo2";
        private const string TestProjectName = "TestProjectX";
        private const string TestProjectDescription = "TestProjectXDescription";
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
        private Mock<ITagApiService> _tagApiServiceMock;

        private ProcosysTagDetails _mainTagDetails1;
        private ProcosysTagDetails _mainTagDetails2;
        
        private CreateTagCommand _command;
        private CreateTagCommandHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            _stepMock = new Mock<Step>();
            _stepMock.SetupGet(x => x.Id).Returns(StepId);
            
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
            var rdMock2 = new Mock<RequirementDefinition>();
            rdMock2.SetupGet(x => x.Id).Returns(ReqDefId2);
            _rtRepositoryMock
                .Setup(r => r.GetRequirementDefinitionsByIdsAsync(new List<int> {ReqDefId1, ReqDefId2}))
                .Returns(Task.FromResult(new List<RequirementDefinition> {rdMock1.Object, rdMock2.Object}));

            _mainTagDetails1 = new ProcosysTagDetails
            {
                AreaCode = "AreaCode1",
                CallOffNo = "CalloffNo1",
                CommPkgNo = "CommPkgNo1",
                Description = "Description1",
                DisciplineCode = "DisciplineCode1",
                McPkgNo = "McPkgNo1",
                PurchaseOrderNo = "PurchaseOrderNo1",
                TagFunctionCode = "TagFunctionCode1",
                TagNo = TestTagNo1,
                ProjectDescription = TestProjectDescription
            };
            _mainTagDetails2 = new ProcosysTagDetails
            {
                AreaCode = "AreaCode2",
                CallOffNo = "CalloffNo2",
                CommPkgNo = "CommPkgNo2",
                Description = "Description2",
                DisciplineCode = "DisciplineCode2",
                McPkgNo = "McPkgNo2",
                PurchaseOrderNo = "PurchaseOrderNo2",
                TagFunctionCode = "TagFunctionCode2",
                TagNo = TestTagNo2,
                ProjectDescription = TestProjectDescription
            };

            IList<ProcosysTagDetails> mainTagDetailList = new List<ProcosysTagDetails> {_mainTagDetails1, _mainTagDetails2};
            _tagApiServiceMock = new Mock<ITagApiService>();
            _tagApiServiceMock
                .Setup(x => x.GetTagDetails(TestPlant, TestProjectName, new List<string>{TestTagNo1, TestTagNo2}))
                .Returns(Task.FromResult(mainTagDetailList));

            _command = new CreateTagCommand(
                new List<string>{TestTagNo1, TestTagNo2}, 
                TestProjectName,
                _stepMock.Object.Id,
                new List<Requirement>
                {
                    new Requirement(ReqDefId1, Interval1),
                    new Requirement(ReqDefId2, Interval2),
                },
                null);
            
            _dut = new CreateTagCommandHandler(
                _projectRepositoryMock.Object,
                _journeyRepositoryMock.Object,
                _rtRepositoryMock.Object,
                UnitOfWorkMock.Object,
                PlantProviderMock.Object,
                _tagApiServiceMock.Object);
        }

        [TestMethod]
        public async Task HandlingCreateTagCommand_ShouldAddProjectToRepository_WhenProjectNotExists()
        {
            // Arrange
            _projectRepositoryMock
                .Setup(r => r.GetByNameAsync(TestProjectName)).Returns(Task.FromResult((Project)null));

            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(2, result.Data.Count);
            Assert.AreEqual(0, _projectAddedToRepository.Id);
            Assert.AreEqual(TestProjectName, _projectAddedToRepository.Name);
            Assert.AreEqual(TestProjectDescription, _projectAddedToRepository.Description);
        }

        [TestMethod]
        public async Task HandlingCreateTagCommand_ShouldNotAddAnyProjectToRepository_WhenProjectAlreadyExists()
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
        public async Task HandlingCreateTagCommand_ShouldAdd2TagsToNewProject_WhenProjectNotExists()
        {
            // Arrange
            _projectRepositoryMock
                .Setup(r => r.GetByNameAsync(TestProjectName)).Returns(Task.FromResult((Project)null));

            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(2, result.Data.Count);
            
            var tags = _projectAddedToRepository.Tags;
            Assert.AreEqual(2, tags.Count);
            AssertTagProperties(_mainTagDetails1, tags.First());
            AssertTagProperties(_mainTagDetails2, tags.Last());
        }

        [TestMethod]
        public async Task HandlingCreateTagCommand_ShouldAdd2TagsToExistingProject_WhenProjectAlreadyExists()
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
            Assert.AreEqual(2, result.Data.Count);
            
            var tags = project.Tags;
            Assert.AreEqual(2, tags.Count);
            AssertTagProperties(_mainTagDetails1, tags.First());
            AssertTagProperties(_mainTagDetails2, tags.Last());
        }

        [TestMethod]
        public async Task HandlingCreateTagCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);
            
            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        private void AssertTagProperties(ProcosysTagDetails mainTagDetails, Tag tagAddedToProject)
        {
            Assert.AreEqual(mainTagDetails.AreaCode, tagAddedToProject.AreaCode);
            Assert.AreEqual(mainTagDetails.CallOffNo, tagAddedToProject.Calloff);
            Assert.AreEqual(mainTagDetails.CommPkgNo, tagAddedToProject.CommPkgNo);
            Assert.AreEqual(mainTagDetails.DisciplineCode, tagAddedToProject.DisciplineCode);
            Assert.AreEqual(0, tagAddedToProject.Id);
            Assert.AreEqual(false, tagAddedToProject.IsAreaTag);
            Assert.AreEqual(mainTagDetails.McPkgNo, tagAddedToProject.McPkgNo);
            Assert.AreEqual(mainTagDetails.Description, tagAddedToProject.Description);
            Assert.AreEqual(mainTagDetails.PurchaseOrderNo, tagAddedToProject.PurchaseOrderNo);
            Assert.AreEqual("TestPlant", tagAddedToProject.Schema);
            Assert.AreEqual(StepId, tagAddedToProject.StepId);
            Assert.AreEqual(mainTagDetails.TagFunctionCode, tagAddedToProject.TagFunctionCode);
            Assert.AreEqual(mainTagDetails.TagNo, tagAddedToProject.TagNo);
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
