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
        private const int TagId = 1;
        private const string TestProject = "ProjectA";
        private PreserveCommand _requestWithProjectAccessCheckViaTagId;
        private GetTagsQuery _requestWithProjectAccessCheckViaProjectName;
        private Mock<IProjectAccessChecker> _projectAccessCheckerMock;
        private Mock<IProjectHelper> _projectHelperMock;

        [TestInitialize]
        public void Setup()
        {
            _projectAccessCheckerMock = new Mock<IProjectAccessChecker>();
            _projectHelperMock = new Mock<IProjectHelper>();
            _requestWithProjectAccessCheckViaTagId = new PreserveCommand(TagId);
            _requestWithProjectAccessCheckViaProjectName = new GetTagsQuery(TestProject);

            _dut = new ProjectAccessValidator(_projectAccessCheckerMock.Object, _projectHelperMock.Object);
        }

        [TestMethod]
        public async Task ValidateAsync_ShouldReturnTrue_WhenAccessToProjectViaProjectName()
        {
            // Arrange
            _projectAccessCheckerMock.Setup(p => p.HasCurrentUserAccessToProject(TestProject)).Returns(true);

            // act
            var result = await _dut.ValidateAsync(_requestWithProjectAccessCheckViaProjectName);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_ShouldReturnFalse_WhenAccessToProjectViaProjectName()
        {
            // Arrange
            _projectAccessCheckerMock.Setup(p => p.HasCurrentUserAccessToProject(TestProject)).Returns(false);
            
            // act
            var result = await _dut.ValidateAsync(_requestWithProjectAccessCheckViaTagId);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_ShouldReturnTrue_WhenAccessToProjectViaTagId()
        {
            // Arrange
            _projectAccessCheckerMock.Setup(p => p.HasCurrentUserAccessToProject(TestProject)).Returns(true);
            _projectHelperMock.Setup(p => p.GetProjectNameFromTagIdAsync(TagId)).Returns(Task.FromResult(TestProject));

            // act
            var result = await _dut.ValidateAsync(_requestWithProjectAccessCheckViaTagId);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_ShouldReturnFalse_WhenAccessToProjectViaTagId()
        {
            // Arrange
            _projectAccessCheckerMock.Setup(p => p.HasCurrentUserAccessToProject(TestProject)).Returns(false);
            _projectHelperMock.Setup(p => p.GetProjectNameFromTagIdAsync(TagId)).Returns(Task.FromResult(TestProject));

            // act
            var result = await _dut.ValidateAsync(_requestWithProjectAccessCheckViaTagId);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
