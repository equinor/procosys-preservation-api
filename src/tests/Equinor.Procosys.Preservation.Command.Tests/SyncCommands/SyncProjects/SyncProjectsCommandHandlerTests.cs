using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.SyncCommands.SyncProjects;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.MainApi.Project;
using Equinor.Procosys.Preservation.MainApi.Tag;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.SyncCommands.SyncProjects
{
    [TestClass]
    public class SyncProjectsCommandHandlerTests : CommandHandlerTestsBase
    {
        private const string TagNo1 = "TagNo1";
        private const string TagNo2 = "TagNo2";
        private const string ProjectName1 = "Project1";
        private const string OldProjectDescription1 = "OldProject1Description";
        private const string ProjectName2 = "Project2";
        private const string ProjectNameNoAccess = "ProjectNoAccess";
        private const string OldProjectDescription2 = "OldProject2Description";
        private const string ProjectDescriptionNoAccess = "ProjectDescriptionNoAccess";
        private const string NewProjectDescription1 = "NewProject1Description";
        private const string NewProjectDescription2 = "NewProject2Description";

        private Mock<IProjectRepository> _projectRepositoryMock;
        private Mock<ITagApiService> _tagApiServiceMock;
        private Mock<IProjectApiService> _projectApiServiceMock;
        private Mock<IPermissionCache> _permissionCacheMock;

        private ProcosysTagDetails _mainTagDetails1;
        private ProcosysTagDetails _mainTagDetails2;
        
        private SyncProjectsCommand _command;
        private SyncProjectsCommandHandler _dut;
        private Project _project1;
        private Project _project2;
        private Project _projectNoAccess;
        private ProcosysProject _mainProject1;
        private ProcosysProject _mainProject2;
        private ProcosysProject _mainProjectNoAccess;
        private Mock<ILogger<SyncProjectsCommandHandler>> _loggerMock;

        [TestInitialize]
        public void Setup()
        {
            _project1 = new Project(TestPlant, ProjectName1, OldProjectDescription1);
            _project2 = new Project(TestPlant, ProjectName2, OldProjectDescription2);
            _projectNoAccess = new Project(TestPlant, ProjectNameNoAccess, ProjectDescriptionNoAccess);
            _projectRepositoryMock = new Mock<IProjectRepository>();
            _projectRepositoryMock
                .Setup(p => p.GetAllProjectsOnlyAsync())
                .Returns(Task.FromResult(new List<Project>{ _project1, _project2, _projectNoAccess}));

            _mainProject1 = new ProcosysProject
            {
                Name = ProjectName1,
                Description = NewProjectDescription1,
                IsClosed = true
            };
            _mainProject2 = new ProcosysProject
            {
                Name = ProjectName2,
                Description = NewProjectDescription2,
                IsClosed = false
            };
            _mainProjectNoAccess = new ProcosysProject
            {
                Name = ProjectNameNoAccess,
                Description = ProjectDescriptionNoAccess,
                IsClosed = true
            };

            _projectRepositoryMock
                .Setup(p => p.GetStandardTagsInProjectOnlyAsync(ProjectName1))
                .Returns(Task.FromResult(new List<Tag>()));

            _projectRepositoryMock
                .Setup(p => p.GetStandardTagsInProjectOnlyAsync(ProjectName2))
                .Returns(Task.FromResult(new List<Tag>()));

            _projectApiServiceMock = new Mock<IProjectApiService>();
            _projectApiServiceMock
                .Setup(x => x.GetProjectAsync(TestPlant, ProjectName1))
                .Returns(Task.FromResult(_mainProject1));
            _projectApiServiceMock
                .Setup(x => x.GetProjectAsync(TestPlant, ProjectName2))
                .Returns(Task.FromResult(_mainProject2));
            _projectApiServiceMock
                .Setup(x => x.GetProjectAsync(TestPlant, ProjectNameNoAccess))
                .Returns(Task.FromResult(_mainProjectNoAccess));

            _permissionCacheMock = new Mock<IPermissionCache>();
            _permissionCacheMock.Setup(p => p.GetProjectNamesForUserOidAsync(TestPlant, CurrentUserOid))
                .Returns(Task.FromResult<IList<string>>(new List<string> {ProjectName1, ProjectName2}));

            _mainTagDetails1 = new ProcosysTagDetails
            {
                AreaCode = "AreaCode1",
                AreaDescription = "AreaDescription1",
                CallOffNo = "CalloffNo1",
                CommPkgNo = "CommPkgNo1",
                Description = "Description1",
                DisciplineCode = "DisciplineCode1",
                DisciplineDescription = "DisciplineDescription1",
                McPkgNo = "McPkgNo1",
                PurchaseOrderNo = "PurchaseOrderNo1",
                TagFunctionCode = "TagFunctionCode1",
                TagNo = TagNo1,
                ProjectDescription = OldProjectDescription1
            };
            _mainTagDetails2 = new ProcosysTagDetails
            {
                AreaCode = "AreaCode2",
                AreaDescription = "AreaDescription2",
                CallOffNo = "CalloffNo2",
                CommPkgNo = "CommPkgNo2",
                Description = "Description2",
                DisciplineCode = "DisciplineCode2",
                DisciplineDescription = "DisciplineDescription1",
                McPkgNo = "McPkgNo2",
                PurchaseOrderNo = "PurchaseOrderNo2",
                TagFunctionCode = "TagFunctionCode2",
                TagNo = TagNo2,
                ProjectDescription = OldProjectDescription1
            };

            IList<ProcosysTagDetails> mainTagDetailList = new List<ProcosysTagDetails> {_mainTagDetails1, _mainTagDetails2};
            _tagApiServiceMock = new Mock<ITagApiService>();
            _tagApiServiceMock
                .Setup(x => x.GetTagDetailsAsync(TestPlant, ProjectName1, new List<string>{TagNo1, TagNo2}))
                .Returns(Task.FromResult(mainTagDetailList));

            _command = new SyncProjectsCommand();

            _loggerMock = new Mock<ILogger<SyncProjectsCommandHandler>>();
            _dut = new SyncProjectsCommandHandler(
                _projectRepositoryMock.Object,
                UnitOfWorkMock.Object,
                PlantProviderMock.Object,
                _projectApiServiceMock.Object,
                _tagApiServiceMock.Object,
                _permissionCacheMock.Object,
                CurrentUserProviderMock.Object,
                _loggerMock.Object);
        }

        [TestMethod]
        public async Task HandlingSyncProjectsCommand_ShouldUpdateProjects()
        {
            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            AssertProjectProperties(_mainProject1, _project1);
            AssertProjectProperties(_mainProject2, _project2);
        }

        [TestMethod]
        public async Task HandlingSyncProjectsCommand_ShouldNotUpdateProject_ForProjectWithoutAccess()
        {
            // Act
            await _dut.Handle(_command, default);

            // Assert
            _projectApiServiceMock.Verify(x => x.GetProjectAsync(TestPlant, ProjectNameNoAccess), Times.Never);
            Assert.AreEqual(ProjectDescriptionNoAccess, _projectNoAccess.Description);
            Assert.IsFalse(_projectNoAccess.IsClosed);
        }

        [TestMethod]
        public async Task HandlingSyncProjectsCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);
            
            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        private void AssertProjectProperties(ProcosysProject mainProject, Project project)
        {
            Assert.AreEqual(mainProject.Description, project.Description);
            Assert.AreEqual(mainProject.IsClosed, project.IsClosed);
        }

        private void AssertTagProperties(ProcosysTagDetails mainTag, Tag tag)
        {
            Assert.AreEqual(mainTag.AreaCode, tag.AreaCode);
            Assert.AreEqual(mainTag.AreaDescription, tag.AreaDescription);
            Assert.AreEqual(mainTag.CallOffNo, tag.Calloff);
            Assert.AreEqual(mainTag.CommPkgNo, tag.CommPkgNo);
            Assert.AreEqual(mainTag.DisciplineCode, tag.DisciplineCode);
            Assert.AreEqual(mainTag.DisciplineDescription, tag.DisciplineDescription);
            Assert.AreEqual(TagType.Standard, tag.TagType);
            Assert.AreEqual(mainTag.McPkgNo, tag.McPkgNo);
            Assert.AreEqual(mainTag.Description, tag.Description);
            Assert.AreEqual(mainTag.PurchaseOrderNo, tag.PurchaseOrderNo);
            Assert.AreEqual(TestPlant, tag.Plant);
            Assert.AreEqual(mainTag.TagFunctionCode, tag.TagFunctionCode);
            Assert.AreEqual(mainTag.TagNo, tag.TagNo);
        }
    }
}
