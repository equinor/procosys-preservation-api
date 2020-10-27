using System.Collections.Generic;
using System.Security.Claims;
using Equinor.Procosys.Preservation.WebApi.Authorizations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.WebApi.Tests.Authorizations
{
    [TestClass]
    public class ClaimsExtensionTests
    {
        [TestMethod]
        public void TryGetOid_ShouldReturnGuid_WhenOidClaimExists()
        {
            // Arrange
            var oid = "50e2322b-1990-42f4-86ac-179c7c075574";
            var claims = new List<Claim> {new Claim(ClaimsExtensions.Oid, oid)};
            
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
    }
}
