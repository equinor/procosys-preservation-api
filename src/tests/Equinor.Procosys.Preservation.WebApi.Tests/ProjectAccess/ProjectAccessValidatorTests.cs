using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.TagCommands.Preserve;
using Equinor.Procosys.Preservation.Query.GetTags;
using Equinor.Procosys.Preservation.WebApi.ProjectAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.WebApi.Tests.ProjectAccess
{
    [TestClass]
    public class ProjectAccessValidatorTests
    {
        private ProjectAccessValidator _dut;
        private const int TagIdWithAccess = 1;
        private const int TagIdWithoutAccess = 2;
        private const string TestProjectWithAccess = "TestProjectWithAccess";
        private const string TestProjectWithoutAccess = "TestProjectWithoutAccess";

        [TestInitialize]
        public void Setup()
        {
            var projectAccessCheckerMock = new Mock<IProjectAccessChecker>();
            var projectHelperMock = new Mock<IProjectHelper>();

            _dut = new ProjectAccessValidator(projectAccessCheckerMock.Object, projectHelperMock.Object);
            projectAccessCheckerMock.Setup(p => p.HasCurrentUserAccessToProject(TestProjectWithoutAccess)).Returns(false);
            projectAccessCheckerMock.Setup(p => p.HasCurrentUserAccessToProject(TestProjectWithAccess)).Returns(true);
            projectHelperMock.Setup(p => p.GetProjectNameFromTagIdAsync(TagIdWithAccess)).Returns(Task.FromResult(TestProjectWithAccess));
            projectHelperMock.Setup(p => p.GetProjectNameFromTagIdAsync(TagIdWithoutAccess)).Returns(Task.FromResult(TestProjectWithoutAccess));
        }

        [TestMethod]
        public async Task ValidateAsync_OnGetTagsQuery_ShouldReturnTrue_WhenAccessToProject()
        {
            var getTagsQuery = new GetTagsQuery(TestProjectWithAccess);
            // act
            var result = await _dut.ValidateAsync(getTagsQuery);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnGetTagsQuery_ShouldReturnFalse_WhenNoAccessToProject()
        {
            var getTagsQuery = new GetTagsQuery(TestProjectWithoutAccess);
            // act
            var result = await _dut.ValidateAsync(getTagsQuery);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnPreserveCommand_ShouldReturnTrue_WhenAccessToProject()
        {
            // Arrange
            var preserveCommand = new PreserveCommand(TagIdWithAccess);
            
            // act
            var result = await _dut.ValidateAsync(preserveCommand);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnPreserveCommand_ShouldReturnFalse_WhenNoAccessToProject()
        {
            // Arrange
            var preserveCommand = new PreserveCommand(TagIdWithoutAccess);
            
            // act
            var result = await _dut.ValidateAsync(preserveCommand);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
