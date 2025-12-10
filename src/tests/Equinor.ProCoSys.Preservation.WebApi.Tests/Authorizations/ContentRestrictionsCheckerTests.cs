using System.Security.Claims;
using Equinor.ProCoSys.Auth.Authorization;
using Equinor.ProCoSys.Auth.Misc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.WebApi.Tests.Authorizations
{
    [TestClass]
    public class RestrictionRolesCheckerTests
    {
        public readonly string RestrictionRole = "R1";
        private RestrictionRolesChecker _dut;
        private ClaimsIdentity _claimsIdentity;
        private Claim _normalRestrictionRoleClaim;
        private Claim _explicitNoRestrictionsClaim;

        [TestInitialize]
        public void Setup()
        {
            _normalRestrictionRoleClaim = new Claim(ClaimTypes.UserData, ClaimsTransformation.GetRestrictionRoleClaimValue(RestrictionRole));
            _explicitNoRestrictionsClaim = new Claim(ClaimTypes.UserData, ClaimsTransformation.GetRestrictionRoleClaimValue(ClaimsTransformation.NoRestrictions));

            var principal = new ClaimsPrincipal();
            _claimsIdentity = new ClaimsIdentity();
            principal.AddIdentity(_claimsIdentity);
            var claimsPrincipalProviderMock = new Mock<IClaimsPrincipalProvider>();
            claimsPrincipalProviderMock.Setup(u => u.GetCurrentClaimsPrincipal()).Returns(principal);

            _dut = new RestrictionRolesChecker(claimsPrincipalProviderMock.Object);
        }

        [TestMethod]
        public void HasCurrentUserExplicitNoRestrictions_ShouldReturnFalse_WhenNoClaims()
        {
            // Act
            var result = _dut.HasCurrentUserExplicitNoRestrictions();

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void HasCurrentUserExplicitNoRestrictions_ShouldReturnFalse_WhenNormalRestrictionClaimOnlyExist()
        {
            // Arrange
            _claimsIdentity.AddClaim(_normalRestrictionRoleClaim);

            // Act
            var result = _dut.HasCurrentUserExplicitNoRestrictions();

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void HasCurrentUserExplicitNoRestrictions_ShouldReturnTrue_WhenNoRestrictionClaimExistsAlone()
        {
            // Arrange
            _claimsIdentity.AddClaim(_explicitNoRestrictionsClaim);

            // Act
            var result = _dut.HasCurrentUserExplicitNoRestrictions();

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void HasCurrentUserExplicitNoRestrictions_ShouldReturnFalse_WhenNoRestrictionClaimExistTogetherWithNormalRestrictionClaim()
        {
            // Arrange (invalid state)
            _claimsIdentity.AddClaim(_explicitNoRestrictionsClaim);
            _claimsIdentity.AddClaim(_normalRestrictionRoleClaim);

            // Act
            var result = _dut.HasCurrentUserExplicitNoRestrictions();

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void HasCurrentUserExplicitAccessToContent_ShouldReturnTrue_WhenNormalRestrictionClaimExists()
        {
            _claimsIdentity.AddClaim(_normalRestrictionRoleClaim);

            // Act
            var result = _dut.HasCurrentUserExplicitAccessToContent(RestrictionRole);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void HasCurrentUserExplicitAccessToContent_ShouldReturnFalse_WhenNormalRestrictionClaimNotExists()
        {
            _claimsIdentity.AddClaim(_normalRestrictionRoleClaim);

            // Act
            var result = _dut.HasCurrentUserExplicitAccessToContent("XYZ");

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void HasCurrentUserExplicitAccessToContent_ShouldReturnFalse_WhenRestrictionToCheckNotGiven()
        {
            _claimsIdentity.AddClaim(_normalRestrictionRoleClaim);

            // Act
            var result1 = _dut.HasCurrentUserExplicitAccessToContent(null);
            var result2 = _dut.HasCurrentUserExplicitAccessToContent("");

            // Assert
            Assert.IsFalse(result1);
            Assert.IsFalse(result2);
        }
    }
}
