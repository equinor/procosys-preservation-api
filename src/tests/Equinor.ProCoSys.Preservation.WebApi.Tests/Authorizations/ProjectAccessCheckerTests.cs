using System.Security.Claims;
using Equinor.ProCoSys.Preservation.WebApi.Authorizations;
using Equinor.ProCoSys.Preservation.WebApi.Misc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.WebApi.Tests.Authorizations
{
    [TestClass]
    public class ProjectAccessCheckerTests
    {
        private readonly string ProjectName = "P1";
        private ProjectAccessChecker _dut;

        [TestInitialize]
        public void Setup()
        {
            var principal = new ClaimsPrincipal();
            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(ClaimTypes.UserData, ClaimsTransformation.GetProjectClaimValue(ProjectName)));
            principal.AddIdentity(claimsIdentity);
            var claimsProviderMock = new Mock<IClaimsProvider>();
            claimsProviderMock.Setup(c => c.GetCurrentUser()).Returns(principal);
            
            _dut = new ProjectAccessChecker(claimsProviderMock.Object);
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
            var result = _dut.HasCurrentUserAccessToProject(ProjectName);

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
