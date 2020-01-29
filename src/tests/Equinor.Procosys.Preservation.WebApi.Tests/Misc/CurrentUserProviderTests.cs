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
        private Person _user;
        private Mock<IHttpContextAccessor> _accessorMock;
        private Mock<IPersonRepository> _personRepositoryMock;
        private CurrentUserProvider _dut;

        [TestInitialize]
        public void Setup()
        {
            _okOid = new Guid("7DFC890F-F82B-4E2D-B81B-41D6C103F83B");

            _user = new Person(_okOid, "Test", "User");

            _accessorMock = new Mock<IHttpContextAccessor>();

            _personRepositoryMock = new Mock<IPersonRepository>();
            _personRepositoryMock
                .Setup(x => x.GetByOidAsync(It.Is<Guid>(g => g == _okOid)))
                .Returns(Task.FromResult(_user));

            _dut = new CurrentUserProvider(_accessorMock.Object, _personRepositoryMock.Object);
        }

        [TestMethod]
        public async Task GetCurrentUserAsync_ReturnsUser_WhenOidExists()
        {
            _accessorMock
                .Setup(x => x.HttpContext.User.Claims)
                .Returns(new List<Claim> { new Claim(OidKey, _okOid.ToString()) });

            var currentUser = await _dut.GetCurrentUserAsync();

            Assert.AreEqual(_user, currentUser);
        }

        [TestMethod]
        public async Task GetCurrentUserAsync_ReturnsNull_WhenOidDoesNotExist()
        {
            var wrongOid = new Guid("12345678-1234-1234-1234-123456789012");

            _accessorMock
                .Setup(x => x.HttpContext.User.Claims)
                .Returns(new List<Claim> { new Claim(OidKey, wrongOid.ToString()) });

            var currentUser = await _dut.GetCurrentUserAsync();

            Assert.IsNull(currentUser);
        }
    }
}
