using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
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
        private Guid _okOid;
        private Mock<IHttpContextAccessor> _accessorMock;
        private CurrentUserProvider _dut;

        [TestInitialize]
        public void Setup()
        {
            _okOid = new Guid("7DFC890F-F82B-4E2D-B81B-41D6C103F83B");
            _accessorMock = new Mock<IHttpContextAccessor>();
            _dut = new CurrentUserProvider(_accessorMock.Object);
        }

        [TestMethod]
        public void GetCurrentUser_ReturnsUser_WhenOidExists()
        {
            _accessorMock
                .Setup(x => x.HttpContext.User.Claims)
                .Returns(new List<Claim> { new Claim(OidKey, _okOid.ToString()) });

            var currentUser = _dut.GetCurrentUser();

            Assert.AreEqual(_okOid, currentUser);
        }

        [TestMethod]
        public void GetCurrentUser_ThrowsException_WhenOidDoesNotExist()
        {
            var illegalOid = "This is not a valid GUID";

            _accessorMock
                .Setup(x => x.HttpContext.User.Claims)
                .Returns(new List<Claim> { new Claim(OidKey, illegalOid) });

            Assert.ThrowsException<Exception>(() => _dut.GetCurrentUser());
        }
    }
}
