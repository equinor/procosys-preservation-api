using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.TagCommands.CreateAreaTag;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.MainApi.Area;
using Equinor.ProCoSys.Preservation.MainApi.Discipline;
using Equinor.ProCoSys.Preservation.MainApi.Project;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.TagCommands.CreateAreaTag
{
    [TestClass]
    public class CreateAreaTagCommandHandlerTests : CommandHandlerTestsBase
    {
        private const string TestProjectName = "TestProjectX";
        private const string TestPurchaseOrder = "TestPurchaseOrder";
        private const string TestCalloff = "TestCalloff";
        private const int StepId = 11;
        private const int ReqDefId1 = 99;
        private const int ReqDefId2 = 199;
        private const int Interval1 = 2;
        private const int Interval2 = 3;
        private const string DisciplineDescription = "DisciplineDescription";
        private const string AreaDescription = "AreaDescription";

        private Mock<Step> _stepMock;
        private Mock<IJourneyRepository> _journeyRepositoryMock;
        private Project _projectAddedToRepository;
        private Mock<IProjectRepository> _projectRepositoryMock;
        private Mock<IRequirementTypeRepository> _rtRepositoryMock;
        private Mock<IProjectApiService> _projectApiServiceMock;
        private Mock<IDisciplineApiService> _disciplineApiServiceMock;
        private Mock<IAreaApiService> _areaApiServiceMock;

        private CreateAreaTagCommand _createPreAreaCommand;
        private CreateAreaTagCommand _createSiteAreaCommand;
        private CreateAreaTagCommand _createPoAreaWithPurchasedOrderCommand;
        private CreateAreaTagCommand _createPoAreaWithCalloffCommand;
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

            _projectApiServiceMock = new Mock<IProjectApiService>();
            _projectApiServiceMock.Setup(s => s.TryGetProjectAsync(TestPlant, TestProjectName))
                .Returns(Task.FromResult(new ProCoSysProject {Description = "ProjectDescription"}));

            var disciplineCode = "D";
            _disciplineApiServiceMock = new Mock<IDisciplineApiService>();
            _disciplineApiServiceMock.Setup(s => s.TryGetDisciplineAsync(TestPlant, disciplineCode))
                .Returns(Task.FromResult(new PCSDiscipline
                {
                    Code = disciplineCode,
                    Description = DisciplineDescription
                }));

            var areaCode = "A";
            _areaApiServiceMock = new Mock<IAreaApiService>();
            _areaApiServiceMock.Setup(s => s.TryGetAreaAsync(TestPlant, areaCode))
                .Returns(Task.FromResult(new PCSArea
                {
                    Code = areaCode,
                    Description = AreaDescription
                }));

            _createPreAreaCommand = new CreateAreaTagCommand(
                TestProjectName,
                TagType.PreArea,
                disciplineCode,
                areaCode,
                "notused",
                null,
                StepId,
                new List<RequirementForCommand>
                {
                    new RequirementForCommand(ReqDefId1, Interval1),
                    new RequirementForCommand(ReqDefId2, Interval2),
                },
                null,
                "Remark",
                "SA");
            _createSiteAreaCommand = new CreateAreaTagCommand(
                TestProjectName,
                TagType.SiteArea,
                disciplineCode,
                areaCode,
                "notused",
                null,
                StepId,
                new List<RequirementForCommand>
                {
                    new RequirementForCommand(ReqDefId1, Interval1),
                    new RequirementForCommand(ReqDefId2, Interval2),
                },
                null,
                "Remark",
                "SA");
            _createPoAreaWithPurchasedOrderCommand = new CreateAreaTagCommand(
                TestProjectName,
                TagType.PoArea,
                disciplineCode,
                areaCode,
                TestPurchaseOrder,
                null,
                StepId,
                new List<RequirementForCommand>
                {
                    new RequirementForCommand(ReqDefId1, Interval1),
                    new RequirementForCommand(ReqDefId2, Interval2),
                },
                null,
                "Remark",
                "SA");
            _createPoAreaWithCalloffCommand = new CreateAreaTagCommand(
                TestProjectName,
                TagType.PoArea,
                disciplineCode,
                areaCode,
                $"{TestPurchaseOrder}/{TestCalloff}",
                null,
                StepId,
                new List<RequirementForCommand>
                {
                    new RequirementForCommand(ReqDefId1, Interval1),
                    new RequirementForCommand(ReqDefId2, Interval2),
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
            // Act
            var result = await _dut.Handle(_createPreAreaCommand, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(TestProjectName, _projectAddedToRepository.Name);
            Assert.AreEqual("ProjectDescription", _projectAddedToRepository.Description);
        }

        [TestMethod]
        public async Task HandlingCreateAreaTagCommand_ShouldNotAddAnyProjectToRepository_WhenProjectAlreadyExists()
        {
            // Arrange
            var project = new Project(TestPlant, TestProjectName, "", _projectProCoSysGuid);
            _projectRepositoryMock
                .Setup(r => r.GetProjectOnlyByNameAsync(TestProjectName)).Returns(Task.FromResult(project));

            // Act
            var result = await _dut.Handle(_createPreAreaCommand, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.IsNull(_projectAddedToRepository);
        }

        [TestMethod]
        public async Task HandlingCreateAreaTagCommand_ShouldAddTagToNewProject_WhenProjectNotExists()
        {
            // Act
            var result = await _dut.Handle(_createPreAreaCommand, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            
            var tags = _projectAddedToRepository.Tags;
            Assert.AreEqual(1, tags.Count);
        }

        [TestMethod]
        public async Task HandlingCreateAreaTagCommand_ShouldAddTagToExistingProject_WhenProjectAlreadyExists()
        {
            // Arrange
            var project = new Project(TestPlant, TestProjectName, "", _projectProCoSysGuid);
            _projectRepositoryMock
                .Setup(r => r.GetProjectOnlyByNameAsync(TestProjectName)).Returns(Task.FromResult(project));
            Assert.AreEqual(0, project.Tags.Count);
            
            // Act
            var result = await _dut.Handle(_createPreAreaCommand, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            
            var tags = project.Tags;
            Assert.AreEqual(1, tags.Count);
        }

        [TestMethod]
        public async Task HandlingCreateAreaTagCommand_ShouldSetCorrectProperties_WhenCreatingPreAreaTag()
        {
            // Act
            var result = await _dut.Handle(_createPreAreaCommand, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            
            var tags = _projectAddedToRepository.Tags;
            Assert.AreEqual(1, tags.Count);
            AssertTagProperties(_createPreAreaCommand, tags.First(), null, null);
        }

        [TestMethod]
        public async Task HandlingCreateAreaTagCommand_ShouldSetCorrectProperties_WhenCreatingSiteAreaTag()
        {
            // Act
            var result = await _dut.Handle(_createSiteAreaCommand, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            
            var tags = _projectAddedToRepository.Tags;
            Assert.AreEqual(1, tags.Count);
            AssertTagProperties(_createSiteAreaCommand, tags.First(), null, null);
        }

        [TestMethod]
        public async Task HandlingCreateAreaTagCommand_ShouldSetCorrectProperties_WhenCreatingPoAreaTagWithJustPo()
        {
            // Act
            var result = await _dut.Handle(_createPoAreaWithPurchasedOrderCommand, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            
            var tags = _projectAddedToRepository.Tags;
            Assert.AreEqual(1, tags.Count);
            AssertTagProperties(_createPoAreaWithPurchasedOrderCommand, tags.First(), TestPurchaseOrder, null);
        }

        [TestMethod]
        public async Task HandlingCreateAreaTagCommand_ShouldSetCorrectProperties_WhenCreatingPoAreaTagWithPoAndCo()
        {
            // Act
            var result = await _dut.Handle(_createPoAreaWithCalloffCommand, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            
            var tags = _projectAddedToRepository.Tags;
            Assert.AreEqual(1, tags.Count);
            AssertTagProperties(_createPoAreaWithCalloffCommand, tags.First(), TestPurchaseOrder, TestCalloff);
        }

        [TestMethod]
        public async Task HandlingCreateAreaTagCommand_ShouldSave()
        {
            _projectRepositoryMock
                .Setup(r => r.GetProjectOnlyByNameAsync(TestProjectName)).Returns(Task.FromResult(new Project(TestPlant, TestProjectName, "", _projectProCoSysGuid)));
            // Act
            await _dut.Handle(_createPreAreaCommand, default);
            
            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        private void AssertTagProperties(CreateAreaTagCommand command, Tag tagAddedToProject, string expectedPurchaseOrder, string expectedCalloff)
        {
            Assert.AreEqual(command.AreaCode, tagAddedToProject.AreaCode);
            Assert.AreEqual(AreaDescription, tagAddedToProject.AreaDescription);
            Assert.AreEqual(expectedCalloff, tagAddedToProject.Calloff);
            Assert.IsNull(tagAddedToProject.CommPkgNo);
            Assert.IsNull(tagAddedToProject.CommPkgProCoSysGuid);
            Assert.AreEqual(command.DisciplineCode, tagAddedToProject.DisciplineCode);
            Assert.AreEqual(DisciplineDescription, tagAddedToProject.DisciplineDescription);
            Assert.AreEqual(command.TagType, tagAddedToProject.TagType);
            Assert.IsNull(tagAddedToProject.McPkgNo);
            Assert.IsNull(tagAddedToProject.McPkgProCoSysGuid);
            Assert.AreEqual(command.Description, tagAddedToProject.Description);
            Assert.AreEqual(command.Remark, tagAddedToProject.Remark);
            Assert.AreEqual(command.StorageArea, tagAddedToProject.StorageArea);
            Assert.AreEqual(expectedPurchaseOrder, tagAddedToProject.PurchaseOrderNo);
            Assert.AreEqual(TestPlant, tagAddedToProject.Plant);
            Assert.AreEqual(StepId, tagAddedToProject.StepId);
            Assert.IsNull(tagAddedToProject.TagFunctionCode);
            Assert.AreEqual(command.GetTagNo(), tagAddedToProject.TagNo);
            Assert.IsNull(tagAddedToProject.ProCoSysGuid);
            Assert.AreEqual(2, tagAddedToProject.Requirements.Count);
            AssertReqProperties(tagAddedToProject.Requirements.First(), ReqDefId1, Interval1);
            AssertReqProperties(tagAddedToProject.Requirements.Last(), ReqDefId2, Interval2);
        }

        private void AssertReqProperties(TagRequirement req, int reqDefId, int interval)
        {
            Assert.AreEqual(reqDefId, req.RequirementDefinitionId);
            Assert.AreEqual(interval, req.IntervalWeeks);
        }
    }
}
