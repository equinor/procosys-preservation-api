using System.Security.Claims;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.WebApi.Authorizations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.WebApi.Tests.Authorizations
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
            var currentUserProviderMock = new Mock<ICurrentUserProvider>();
            currentUserProviderMock.Setup(u => u.GetCurrentUser()).Returns(principal);
            
            _dut = new ProjectAccessChecker(currentUserProviderMock.Object);
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
    }
}
