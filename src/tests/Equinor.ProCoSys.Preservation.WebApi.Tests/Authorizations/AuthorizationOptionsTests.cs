using System;
using System.Linq;
using Equinor.ProCoSys.Preservation.WebApi.Authorizations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.WebApi.Tests.Authorizations
{
    [TestClass]
    public class AuthorizationOptionsTests
    {
        [TestMethod]
        public void CrossPlantUserOids_ShouldReturnOids_ForCorrectGuids()
        {
            // Arrange
            var oid1 = "50e2322b-1990-42f4-86ac-179c7c075574";
            var oid2 = "{CBA3319A-FF84-4688-8833-834582D81D89}";
            var dut = new AuthorizationOptions {CrossPlantUserOidList = $"{oid1},{oid2}"};
            
            // Act
            var oids = dut.CrossPlantUserOids();

            // Assert
            Assert.AreEqual(2, oids.Count);
            Assert.AreEqual(new Guid(oid1), oids.First());
            Assert.AreEqual(new Guid(oid2), oids.Last());
        }

        [TestMethod]
        public void CrossPlantUserOids_ShouldReturnEmptyList_ForInCorrectGuid()
        {
            // Arrange
            var oid1 = "abc";
            var oid2 = "123";
            var dut = new AuthorizationOptions {CrossPlantUserOidList = $"{oid1},{oid2}"};
            
            // Act
            var oids = dut.CrossPlantUserOids();

            // Assert
            Assert.AreEqual(0, oids.Count);
        }

        [TestMethod]
        public void CrossPlantUserOids_ShouldReturnEmptyList_WhenCrossPlantUserOidListNotSet()
        {
            // Arrange
            var dut = new AuthorizationOptions();
            
            // Act
            var oids = dut.CrossPlantUserOids();

            // Assert
            Assert.AreEqual(0, oids.Count);
        }
    }
}
