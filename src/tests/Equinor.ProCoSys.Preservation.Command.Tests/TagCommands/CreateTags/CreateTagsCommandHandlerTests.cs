using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.TagCommands.CreateTags;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.ProCoSys.Preservation.MainApi.Tag;
using Equinor.ProCoSys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.TagCommands.CreateTags
{
    [TestClass]
    public class CreateTagsCommandHandlerTests : CommandHandlerTestsBase
    {
        private const string TestTagNo1 = "TagNo1";
        private const string TestTagNo2 = "TagNo2";
        private const string TestProjectName = "TestProjectX";
        private const string TestProjectDescription = "TestProjectXDescription";
        private const int ModeId = 2;
        private const int StepId = 11;
        private const int ReqDefId1 = 99;
        private const int ReqDefId2 = 199;
        private const int Interval1 = 2;
        private const int Interval2 = 3;

        private Mock<Mode> _modeMock;
        private Step _step;
        private Mock<IJourneyRepository> _journeyRepositoryMock;
        private Mock<IModeRepository> _modeRepositoryMock;
        private Project _projectAddedToRepository;
        private Mock<IProjectRepository> _projectRepositoryMock;
        private Mock<IRequirementTypeRepository> _rtRepositoryMock;
        private Mock<ITagApiService> _tagApiServiceMock;

        private PCSTagDetails _mainTagDetails1;
        private PCSTagDetails _mainTagDetails2;
        
        private CreateTagsCommand _command;
        private CreateTagsCommandHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            _modeRepositoryMock = new Mock<IModeRepository>();
            _modeMock = new Mock<Mode>();
            _modeMock.SetupGet(m => m.Plant).Returns(TestPlant);
            _modeMock.SetupGet(x => x.Id).Returns(ModeId);
            _modeRepositoryMock
                .Setup(r => r.GetByIdAsync(ModeId))
                .Returns(Task.FromResult(_modeMock.Object));
            
            // Arrange
            _step = new Step(TestPlant, "S", _modeMock.Object, new Responsible(TestPlant, "RC", "RD"));
            _step.SetProtectedIdForTesting(StepId);

            _journeyRepositoryMock = new Mock<IJourneyRepository>();
            _journeyRepositoryMock
                .Setup(x => x.GetStepByStepIdAsync(StepId))
                .Returns(Task.FromResult(_step));

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

            _mainTagDetails1 = new PCSTagDetails
            {
                AreaCode = "AreaCode1",
                AreaDescription = "AreaDescription1",
                CallOffNo = "CalloffNo1",
                CommPkgNo = "CommPkgNo1",
                CommPkgProCoSysGuid = Guid.NewGuid(),
                Description = "Description1",
                DisciplineCode = "DisciplineCode1",
                DisciplineDescription = "DisciplineDescription1",
                McPkgNo = "McPkgNo1",
                McPkgProCoSysGuid = Guid.NewGuid(),
                PurchaseOrderNo = "PurchaseOrderNo1",
                TagFunctionCode = "TagFunctionCode1",
                TagNo = TestTagNo1,
                ProCoSysGuid = Guid.NewGuid(),
                ProjectDescription = TestProjectDescription
            };
            _mainTagDetails2 = new PCSTagDetails
            {
                AreaCode = "AreaCode2",
                AreaDescription = "AreaDescription2",
                CallOffNo = "CalloffNo2",
                CommPkgNo = "CommPkgNo2",
                CommPkgProCoSysGuid = Guid.NewGuid(),
                Description = "Description2",
                DisciplineCode = "DisciplineCode2",
                DisciplineDescription = "DisciplineDescription1",
                McPkgNo = "McPkgNo2",
                McPkgProCoSysGuid = Guid.NewGuid(),
                PurchaseOrderNo = "PurchaseOrderNo2",
                TagFunctionCode = "TagFunctionCode2",
                TagNo = TestTagNo2,
                ProCoSysGuid = Guid.NewGuid(),
                ProjectDescription = TestProjectDescription
            };

            IList<PCSTagDetails> mainTagDetailList = new List<PCSTagDetails> {_mainTagDetails1, _mainTagDetails2};
            _tagApiServiceMock = new Mock<ITagApiService>();
            _tagApiServiceMock
                .Setup(x => x.GetTagDetailsAsync(TestPlant, TestProjectName, new List<string>{TestTagNo1, TestTagNo2}))
                .Returns(Task.FromResult(mainTagDetailList));

            _command = new CreateTagsCommand(
                new List<string>{TestTagNo1, TestTagNo2}, 
                TestProjectName,
                _step.Id,
                new List<RequirementForCommand>
                {
                    new RequirementForCommand(ReqDefId1, Interval1),
                    new RequirementForCommand(ReqDefId2, Interval2),
                },
                "Remark",
                "SA");
            
            _dut = new CreateTagsCommandHandler(
                _projectRepositoryMock.Object,
                _journeyRepositoryMock.Object,
                _modeRepositoryMock.Object,
                _rtRepositoryMock.Object,
                UnitOfWorkMock.Object,
                PlantProviderMock.Object,
                _tagApiServiceMock.Object);
        }

        [TestMethod]
        public async Task HandlingCreateTagsCommand_ShouldAddProjectToRepository_WhenProjectNotExists()
        {
            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(2, result.Data.Count);
            Assert.AreEqual(TestProjectName, _projectAddedToRepository.Name);
            Assert.AreEqual(TestProjectDescription, _projectAddedToRepository.Description);
        }

        [TestMethod]
        public async Task HandlingCreateTagsCommand_ShouldNotAddAnyProjectToRepository_WhenProjectAlreadyExists()
        {
            // Arrange
            var project = new Project(TestPlant, TestProjectName, "");
            _projectRepositoryMock.Setup(r => r.GetProjectOnlyByNameAsync(TestProjectName)).Returns(Task.FromResult(project));

            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.IsNull(_projectAddedToRepository);
        }

        [TestMethod]
        public async Task HandlingCreateTagsCommand_ShouldAdd2TagsToNewProject_WhenProjectNotExists()
        {
            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(2, result.Data.Count);
            
            var tags = _projectAddedToRepository.Tags;
            Assert.AreEqual(2, tags.Count);
            AssertTagProperties(_command, _mainTagDetails1, tags.First());
            AssertTagProperties(_command, _mainTagDetails2, tags.Last());
        }

        [TestMethod]
        public async Task HandlingCreateTagsCommand_ShouldAdd2TagsToExistingProject_WhenProjectAlreadyExists()
        {
            // Arrange
            var project = new Project(TestPlant, TestProjectName, "");
            _projectRepositoryMock.Setup(r => r.GetProjectOnlyByNameAsync(TestProjectName)).Returns(Task.FromResult(project));
            Assert.AreEqual(0, project.Tags.Count);
            
            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(2, result.Data.Count);
            
            var tags = project.Tags;
            Assert.AreEqual(2, tags.Count);
            AssertTagProperties(_command, _mainTagDetails1, tags.First());
            AssertTagProperties(_command, _mainTagDetails2, tags.Last());
        }

        [TestMethod]
        public async Task HandlingCreateTagsCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);
            
            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        [TestMethod]
        public async Task HandlingCreateTagsCommand_ShouldReturnNotFound_WhenAddingTagWithOutPoToSupplierStep()
        {
            // Arrange
            _modeMock.Object.ForSupplier = true;
            _mainTagDetails1.PurchaseOrderNo = null;

            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual($"Purchase Order for {_mainTagDetails1.TagNo} not found in project {TestProjectName}. Tag can not be in a Supplier step", result.Errors[0]);
        }

        private void AssertTagProperties(CreateTagsCommand command, PCSTagDetails mainTagDetails, Tag tagAddedToProject)
        {
            Assert.AreEqual(mainTagDetails.AreaCode, tagAddedToProject.AreaCode);
            Assert.AreEqual(mainTagDetails.AreaDescription, tagAddedToProject.AreaDescription);
            Assert.AreEqual(mainTagDetails.CallOffNo, tagAddedToProject.Calloff);
            Assert.AreEqual(mainTagDetails.CommPkgNo, tagAddedToProject.CommPkgNo);
            Assert.IsNotNull(tagAddedToProject.CommPkgProCoSysGuid);
            Assert.AreEqual(mainTagDetails.CommPkgProCoSysGuid, tagAddedToProject.CommPkgProCoSysGuid);
            Assert.AreEqual(mainTagDetails.DisciplineCode, tagAddedToProject.DisciplineCode);
            Assert.AreEqual(mainTagDetails.DisciplineDescription, tagAddedToProject.DisciplineDescription);
            Assert.AreEqual(TagType.Standard, tagAddedToProject.TagType);
            Assert.AreEqual(mainTagDetails.McPkgNo, tagAddedToProject.McPkgNo);
            Assert.IsNotNull(tagAddedToProject.McPkgProCoSysGuid);
            Assert.AreEqual(mainTagDetails.McPkgProCoSysGuid, tagAddedToProject.McPkgProCoSysGuid);
            Assert.AreEqual(mainTagDetails.Description, tagAddedToProject.Description);
            Assert.AreEqual(command.Remark, tagAddedToProject.Remark);
            Assert.AreEqual(command.StorageArea, tagAddedToProject.StorageArea);
            Assert.AreEqual(mainTagDetails.PurchaseOrderNo, tagAddedToProject.PurchaseOrderNo);
            Assert.AreEqual(TestPlant, tagAddedToProject.Plant);
            Assert.AreEqual(StepId, tagAddedToProject.StepId);
            Assert.AreEqual(mainTagDetails.TagFunctionCode, tagAddedToProject.TagFunctionCode);
            Assert.AreEqual(mainTagDetails.TagNo, tagAddedToProject.TagNo);
            Assert.IsNotNull(tagAddedToProject.ProCoSysGuid);
            Assert.AreEqual(mainTagDetails.ProCoSysGuid, tagAddedToProject.ProCoSysGuid);
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
