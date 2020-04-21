using System;
using System.Collections.Generic;
using System.Security.Claims;
using Equinor.Procosys.Preservation.WebApi.Misc;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.WebApi.Tests.Misc
{
    [TestClass]
    public class CurrentUserProviderTests
    {
        private const string OidKey = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        private Guid _okOid = new Guid("7DFC890F-F82B-4E2D-B81B-41D6C103F83B");
        private Mock<IHttpContextAccessor> _accessorMock;
        private CurrentUserProvider _dut;

        [TestInitialize]
        public void Setup()
        {
            _accessorMock = new Mock<IHttpContextAccessor>();
            _accessorMock
                .Setup(x => x.HttpContext.User.Claims)
                .Returns(new List<Claim> { new Claim(OidKey, _okOid.ToString()) });

            _dut = new CurrentUserProvider(_accessorMock.Object);
        }

        [TestMethod]
        public void GetCurrentUserOid_ReturnsOid_WhenOidExists()
        {
            var oid = _dut.GetCurrentUser();

            Assert.AreEqual(_okOid, oid);
        }

        [TestMethod]
        public void TryGetCurrentUserOid_ReturnsOid_WhenOidExists()
        {
            var oid = _dut.TryGetCurrentUserOid();

            Assert.IsTrue(oid.HasValue);
            Assert.AreEqual(_okOid, oid.Value);
        }

        [TestMethod]
        public void GetCurrentUserOid_ThrowsException_WhenOidDoesNotExist()
        {
            var illegalOid = "This is not a valid GUID";

            _accessorMock
                .Setup(x => x.HttpContext.User.Claims)
                .Returns(new List<Claim> { new Claim(OidKey, illegalOid) });

            Assert.ThrowsException<Exception>(() => _dut.GetCurrentUser());
        }

        [TestMethod]
        public void TryGetCurrentUserOid_ReturnsNull_WhenOidDoesNotExist()
        {
            var illegalOid = "This is not a valid GUID";

            _accessorMock
                .Setup(x => x.HttpContext.User.Claims)
                .Returns(new List<Claim> { new Claim(OidKey, illegalOid) });

            var oid = _dut.TryGetCurrentUserOid();

            Assert.IsFalse(oid.HasValue);
        }
    }
}
