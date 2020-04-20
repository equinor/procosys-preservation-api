using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Equinor.Procosys.Preservation.WebApi.Authorizations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.WebApi.Tests.Misc
{
    [TestClass]
    public class ClaimsExtensionTests
    {
        [TestMethod]
        public void TryGetOid_ShouldReturnGuid_WhenOidClaimExists()
        {
            // Arrange
            var oid = "50e2322b-1990-42f4-86ac-179c7c075574";
            var claims = new List<Claim> {new Claim(ClaimsExtensions.OidType, oid)};
            
            // Act
            var guid = claims.TryGetOid();

            // Assert
            Assert.IsTrue(guid.HasValue);
            Assert.AreEqual(oid.ToLower(), guid.Value.ToString().ToLower());
        }

        [TestMethod]
        public void TryGetOid_ShouldReturnNull_WhenOidClaimNotExists()
        {
            // Arrange
            var oid = "50e2322b-1990-42f4-86ac-179c7c075574";
            var claims = new List<Claim> {new Claim(ClaimTypes.UserData, oid)};
            
            // Act
            var guid = claims.TryGetOid();

            // Assert
            Assert.IsFalse(guid.HasValue);
        }

        [TestMethod]
        public void HasProjectClaim_ShouldReturnTrue_WhenProjectClaimExists()
        {
            // Arrange
            var project = "ProjectX";
            var claims = new List<Claim> {new Claim(ClaimTypes.UserData, ClaimsTransformation.GetProjectClaimValue(project))};
            
            // Act
            var hasProjectClaim = claims.HasProjectClaim(project);

            // Assert
            Assert.IsTrue(hasProjectClaim);
        }

        [TestMethod]
        public void HasProjectClaim_ShouldReturnFalse_WhenProjectClaimNotExists()
        {
            // Arrange
            var project = "ProjectX";
            var claims = new List<Claim> {new Claim(ClaimTypes.UserData, ClaimsTransformation.GetProjectClaimValue(project))};
            
            // Act
            var hasProjectClaim = claims.HasProjectClaim("ProjectY");

            // Assert
            Assert.IsFalse(hasProjectClaim);
        }

        [TestMethod]
        public void HasContentRestrictionClaim_ShouldReturnTrue_WhenProjectClaimExists()
        {
            // Arrange
            var contentRestriction = "ContentRestrictionX";
            var claims = new List<Claim> {new Claim(ClaimTypes.UserData, ClaimsTransformation.GetContentRestrictionClaimValue(contentRestriction))};
            
            // Act
            var hasContentRestrictionClaim = claims.HasContentRestrictionClaim(contentRestriction);

            // Assert
            Assert.IsTrue(hasContentRestrictionClaim);
        }

        [TestMethod]
        public void HasContentRestrictionClaim_ShouldReturnFalse_WhenContentRestrictionClaimNotExists()
        {
            // Arrange
            var contentRestriction = "ContentRestrictionX";
            var claims = new List<Claim> {new Claim(ClaimTypes.UserData, ClaimsTransformation.GetContentRestrictionClaimValue(contentRestriction))};
            
            // Act
            var hasContentRestrictionClaim = claims.HasContentRestrictionClaim("ContentRestrictionY");

            // Assert
            Assert.IsFalse(hasContentRestrictionClaim);
        }

        [TestMethod]
        public void ContentRestrictionClaims_ShouldContentRestrictionClaims()
        {
            // Arrange
            var contentRestriction = "X";
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.UserData, ClaimsTransformation.GetContentRestrictionClaimValue(contentRestriction))
            };
            
            // Act
            var contentRestrictionClaims = claims.ContentRestrictionClaims();

            // Assert
            Assert.AreEqual(1, contentRestrictionClaims.Count);
            var claim = contentRestrictionClaims.Single();
            Assert.AreEqual(ClaimsTransformation.GetContentRestrictionClaimValue(contentRestriction), claim.Value);
            Assert.AreEqual(ClaimTypes.UserData, claim.Type);
        }
    }
}
