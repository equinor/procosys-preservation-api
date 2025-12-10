using System.Security.Claims;
using Equinor.ProCoSys.Auth.Authorization;
using Equinor.ProCoSys.Auth.Misc;
using Equinor.ProCoSys.Preservation.WebApi.Authorizations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.WebApi.Tests.Authorizations
{
    [TestClass]
    public class ProjectAccessCheckerTests
    {
        private readonly string _projectName = "P1";
        private ProjectAccessChecker _dut;

        [TestInitialize]
        public void Setup()
        {
            var principal = new ClaimsPrincipal();
            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(ClaimTypes.UserData, ClaimsTransformation.GetProjectClaimValue(_projectName)));
            principal.AddIdentity(claimsIdentity);
            var claimsPrincipalProviderMock = new Mock<IClaimsPrincipalProvider>();
            claimsPrincipalProviderMock.Setup(c => c.GetCurrentClaimsPrincipal()).Returns(principal);

            _dut = new ProjectAccessChecker(claimsPrincipalProviderMock.Object);
        }

        [TestMethod]
        public void HasCurrentUserAccessToProject_ShouldReturnFalse_WhenProjectClaimNotExists()
        {
            // Act
            var result = _dut.HasCurrentUserAccessToProject("XYZ");

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void HasCurrentUserAccessToProject_ShouldReturnTrue_WhenProjectClaimExists()
        {
            // Act
            var result = _dut.HasCurrentUserAccessToProject(_projectName);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void HasCurrentUserAccessToProject_ShouldReturnFalse_WhenProjectToCheckNotGiven()
        {
            // Act
            var result1 = _dut.HasCurrentUserAccessToProject(null);
            var result2 = _dut.HasCurrentUserAccessToProject("");

            // Assert
            Assert.IsFalse(result1);
            Assert.IsFalse(result2);
        }
    }
}
