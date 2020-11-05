using System;
using Equinor.Procosys.Preservation.WebApi.Misc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.WebApi.Tests.Misc
{
    [TestClass]
    public class CurrentUserProviderTests
    {
        [TestMethod]
        public void GetCurrentUserOid_ShouldReturnOid_WhenOidExists()
        {
            var okOid = new Guid("7DFC890F-F82B-4E2D-B81B-41D6C103F83B");
            var dut = new CurrentUserProvider();
            dut.SetCurrentUserOid(okOid);

            var oid = dut.GetCurrentUserOid();

            Assert.AreEqual(okOid, oid);
        }

        [TestMethod]
        public void GetCurrentUserOid_ThrowsException_WhenOidDoesNotExist()
        {
            var dut = new CurrentUserProvider();
            Assert.ThrowsException<Exception>(() => dut.GetCurrentUserOid());
        }

    }
}
