using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.Time;
using Equinor.Procosys.Preservation.Infrastructure.Caching;
using Equinor.Procosys.Preservation.MainApi.Permission;
using Equinor.Procosys.Preservation.Test.Common;
using Equinor.Procosys.Preservation.WebApi.Caches;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.WebApi.Tests.Caches
{
    [TestClass]
    public class PermissionCacheTests
    {
        private PermissionCache _dut;
        private readonly Guid Oid = new Guid("{3BFB54C7-91E2-422E-833F-951AD07FE37F}");
        private Mock<IPermissionApiService> _permissionApiServiceMock;
        private readonly string TestPlant = "TestPlant";
        private readonly string Permission1 = "A";
        private readonly string Permission2 = "B";
        private readonly string Project1 = "P1";
        private readonly string Project2 = "P2";
        private readonly string Restriction1 = "R1";
        private readonly string Restriction2 = "R2";

        [TestInitialize]
        public void Setup()
        {
            TimeService.SetProvider(new ManualTimeProvider(new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc)));

            _permissionApiServiceMock = new Mock<IPermissionApiService>();
            _permissionApiServiceMock.Setup(p => p.GetProjectsAsync(TestPlant))
                .Returns(Task.FromResult<IList<string>>(new List<string> {Project1, Project2}));
            _permissionApiServiceMock.Setup(p => p.GetPermissionsAsync(TestPlant))
                .Returns(Task.FromResult<IList<string>>(new List<string> {Permission1, Permission2}));
            _permissionApiServiceMock.Setup(p => p.GetContentRestrictionsAsync(TestPlant))
                .Returns(Task.FromResult<IList<string>>(new List<string> {Restriction1, Restriction2}));

            var optionsMock = new Mock<IOptionsMonitor<CacheOptions>>();
            optionsMock
                .Setup(x => x.CurrentValue)
                .Returns(new CacheOptions());

            _dut = new PermissionCache(new CacheManager(), _permissionApiServiceMock.Object, optionsMock.Object);
        }

        [TestMethod]
        public async Task GetPermissionsForUserOid_ShouldReturnPermissionsFromPermissionApiServiceFirstTime()
        {
            // Act
            var result = await _dut.GetPermissionsForUserAsync(TestPlant, Oid);

            // Assert
            AssertPermissions(result);
            _permissionApiServiceMock.Verify(p => p.GetPermissionsAsync(TestPlant), Times.Once);
        }

        [TestMethod]
        public async Task GetPermissionsForUserOid_ShouldReturnPermissionsFromCacheSecondTime()
        {
            await _dut.GetPermissionsForUserAsync(TestPlant, Oid);
            // Act
            var result = await _dut.GetPermissionsForUserAsync(TestPlant, Oid);

            // Assert
            AssertPermissions(result);
            // since GetPermissionsForUserOidAsync has been called twice, but GetPermissionsAsync has been called once, the second Get uses cache
            _permissionApiServiceMock.Verify(p => p.GetPermissionsAsync(TestPlant), Times.Once);
        }

        [TestMethod]
        public async Task GetProjectsForUserOid_ShouldReturnProjectsFromPermissionApiServiceFirstTime()
        {
            // Act
            var result = await _dut.GetProjectNamesForUserOidAsync(TestPlant, Oid);

            // Assert
            AssertProjects(result);
            _permissionApiServiceMock.Verify(p => p.GetProjectsAsync(TestPlant), Times.Once);
        }

        [TestMethod]
        public async Task GetProjectsForUserOid_ShouldReturnProjectsFromCacheSecondTime()
        {
            await _dut.GetProjectNamesForUserOidAsync(TestPlant, Oid);
            // Act
            var result = await _dut.GetProjectNamesForUserOidAsync(TestPlant, Oid);

            // Assert
            AssertProjects(result);
            // since GetProjectsForUserOidAsync has been called twice, but GetProjectsAsync has been called once, the second Get uses cache
            _permissionApiServiceMock.Verify(p => p.GetProjectsAsync(TestPlant), Times.Once);
        }

        [TestMethod]
        public async Task GetContentRestrictionsForUserOid_ShouldReturnPermissionsFromPermissionApiServiceFirstTime()
        {
            // Act
            var result = await _dut.GetContentRestrictionsForUserOidAsync(TestPlant, Oid);

            // Assert
            AssertRestrictions(result);
            _permissionApiServiceMock.Verify(p => p.GetContentRestrictionsAsync(TestPlant), Times.Once);
        }

        [TestMethod]
        public async Task GetContentRestrictionsForUserOid_ShouldReturnPermissionsFromCacheSecondTime()
        {
            await _dut.GetContentRestrictionsForUserOidAsync(TestPlant, Oid);
            // Act
            var result = await _dut.GetContentRestrictionsForUserOidAsync(TestPlant, Oid);

            // Assert
            AssertRestrictions(result);
            // since GetContentRestrictionsForUserOidAsync has been called twice, but GetContentRestrictionsAsync has been called once, the second Get uses cache
            _permissionApiServiceMock.Verify(p => p.GetContentRestrictionsAsync(TestPlant), Times.Once);
        }

        [TestMethod]
        public async Task GetPermissionsForUserOid_ShouldThrowExceptionWhenOidIsEmty()
            => await Assert.ThrowsExceptionAsync<Exception>(() => _dut.GetPermissionsForUserAsync(TestPlant, Guid.Empty));

        [TestMethod]
        public async Task GetProjectNamesForUserOid_ShouldThrowExceptionWhenOidIsEmty()
            => await Assert.ThrowsExceptionAsync<Exception>(() => _dut.GetProjectNamesForUserOidAsync(TestPlant, Guid.Empty));

        [TestMethod]
        public async Task GetContentRestrictionsForUserOid_ShouldThrowExceptionWhenOidIsEmty()
            => await Assert.ThrowsExceptionAsync<Exception>(() => _dut.GetContentRestrictionsForUserOidAsync(TestPlant, Guid.Empty));

        [TestMethod]
        public void ClearAll_ShouldClearAllPermissionCaches()
        {
            // Arrange
            var cacheManagerMock = new Mock<ICacheManager>();
            var dut = new PermissionCache(
                cacheManagerMock.Object,
                _permissionApiServiceMock.Object,
                new Mock<IOptionsMonitor<CacheOptions>>().Object);
            // Act
            dut.ClearAll(TestPlant, Oid);

            // Assert
            cacheManagerMock.Verify(c => c.Remove(It.IsAny<string>()), Times.Exactly(3));
        }

        private void AssertPermissions(IList<string> result)
        {
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(Permission1, result.First());
            Assert.AreEqual(Permission2, result.Last());
        }

        private void AssertProjects(IList<string> result)
        {
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(Project1, result.First());
            Assert.AreEqual(Project2, result.Last());
        }

        private void AssertRestrictions(IList<string> result)
        {
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(Restriction1, result.First());
            Assert.AreEqual(Restriction2, result.Last());
        }
    }
}
