using System;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Auth.Caches;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.WebApi.Misc;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.WebApi.Tests.Misc
{
    [TestClass]
    public class ProjectCheckerTests
    {
        private readonly Guid _currentUserOid = new Guid("12345678-1234-1234-1234-123456789123");
        private readonly string Plant = "Plant";
        private readonly string Project = "Project";

        private Mock<IPlantProvider> _plantProviderMock;
        private Mock<ICurrentUserProvider> _currentUserProviderMock;
        private Mock<IPermissionCache> _permissionCacheMock;
        private TestRequest _testRequest;
        private ProjectChecker _dut;

        [TestInitialize]
        public void Setup()
        {
            _plantProviderMock = new Mock<IPlantProvider>();
            _plantProviderMock.SetupGet(p => p.Plant).Returns(Plant);

            _currentUserProviderMock = new Mock<ICurrentUserProvider>();
            _currentUserProviderMock.Setup(c => c.GetCurrentUserOid()).Returns(_currentUserOid);

            _permissionCacheMock = new Mock<IPermissionCache>();

            _testRequest = new TestRequest(Project);
            _dut = new ProjectChecker(_plantProviderMock.Object, _currentUserProviderMock.Object, _permissionCacheMock.Object);
        }

        [TestMethod]
        public async Task EnsureValidProjectAsync_ShouldValidateOK()
        {
            // Arrange
            _permissionCacheMock.Setup(p => p.IsAValidProjectForUserAsync(Plant, _currentUserOid, Project, It.IsAny<CancellationToken>())).Returns(Task.FromResult(true));

            // Act
            await _dut.EnsureValidProjectAsync(_testRequest, It.IsAny<CancellationToken>());
        }

        [TestMethod]
        public async Task EnsureValidProjectAsync_ShouldThrowInvalidException_WhenProjectIsNotValid()
        {
            // Arrange
            _permissionCacheMock.Setup(p => p.IsAValidProjectForUserAsync(Plant, _currentUserOid, Project, It.IsAny<CancellationToken>())).Returns(Task.FromResult(false));

            // Act
            await Assert.ThrowsExceptionAsync<InValidProjectException>(() => _dut.EnsureValidProjectAsync(_testRequest, It.IsAny<CancellationToken>()));
        }

        [TestMethod]
        public async Task EnsureValidProjectAsync_ShouldThrowException_WhenRequestIsNull()
            => await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _dut.EnsureValidProjectAsync((IBaseRequest)null, It.IsAny<CancellationToken>()));

        private class TestRequest : IBaseRequest, IProjectRequest
        {
            public TestRequest(string projectName) => ProjectName = projectName;

            public string ProjectName { get; }
        }
    }
}
