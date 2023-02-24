using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.Time;
using Equinor.ProCoSys.Preservation.Infrastructure.Caching;
using Equinor.ProCoSys.Preservation.MainApi.Me;
using Equinor.ProCoSys.Preservation.MainApi.Permission;
using Equinor.ProCoSys.Preservation.Test.Common;
using Equinor.ProCoSys.Preservation.WebApi.Caches;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.WebApi.Tests.Caches
{
    [TestClass]
    public class PermissionCacheTests
    {
        private PermissionCache _dut;
        private readonly Guid Oid = new Guid("{3BFB54C7-91E2-422E-833F-951AD07FE37F}");
        private Mock<IMeApiService> _meApiServiceMock;
        private Mock<IPermissionApiService> _permissionApiServiceMock;
        private readonly string TestPlant = "TestPlant";
        private readonly string Permission1 = "A";
        private readonly string Permission2 = "B";
        private readonly string Project1WithAccess = "P1";
        private readonly string Project2WithAccess = "P2";
        private readonly string ProjectWithoutAccess = "P3";
        private readonly string Restriction1 = "R1";
        private readonly string Restriction2 = "R2";

        [TestInitialize]
        public void Setup()
        {
            TimeService.SetProvider(new ManualTimeProvider(new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc)));

            _meApiServiceMock = new Mock<IMeApiService>();
            _permissionApiServiceMock = new Mock<IPermissionApiService>();
            _permissionApiServiceMock.Setup(p => p.GetAllOpenProjectsAsync(TestPlant))
                .Returns(Task.FromResult<IList<ProCoSysProject>>(new List<ProCoSysProject>
                {
                    new ProCoSysProject {Name = Project1WithAccess, HasAccess = true},
                    new ProCoSysProject {Name = Project2WithAccess, HasAccess = true},
                    new ProCoSysProject {Name = ProjectWithoutAccess}
                }));
            _permissionApiServiceMock.Setup(p => p.GetPermissionsAsync(TestPlant))
                .Returns(Task.FromResult<IList<string>>(new List<string> {Permission1, Permission2}));
            _permissionApiServiceMock.Setup(p => p.GetContentRestrictionsAsync(TestPlant))
                .Returns(Task.FromResult<IList<string>>(new List<string> {Restriction1, Restriction2}));

            var optionsMock = new Mock<IOptionsSnapshot<CacheOptions>>();
            optionsMock
                .Setup(x => x.Value)
                .Returns(new CacheOptions());

            _dut = new PermissionCache(
                new CacheManager(),
                _permissionApiServiceMock.Object,
                _meApiServiceMock.Object,
                optionsMock.Object);
        }

        [TestMethod]
        public async Task GetPermissionsForUser_ShouldReturnPermissionsFromPermissionApiServiceFirstTime()
        {
            // Act
            var result = await _dut.GetPermissionsForUserAsync(TestPlant, Oid);

            // Assert
            AssertPermissions(result);
            _permissionApiServiceMock.Verify(p => p.GetPermissionsAsync(TestPlant), Times.Once);
        }

        [TestMethod]
        public async Task GetPermissionsForUser_ShouldReturnPermissionsFromCacheSecondTime()
        {
            await _dut.GetPermissionsForUserAsync(TestPlant, Oid);
            // Act
            var result = await _dut.GetPermissionsForUserAsync(TestPlant, Oid);

            // Assert
            AssertPermissions(result);
            // since GetPermissionsForUserAsync has been called twice, but GetPermissionsAsync has been called once, the second Get uses cache
            _permissionApiServiceMock.Verify(p => p.GetPermissionsAsync(TestPlant), Times.Once);
        }

        [TestMethod]
        public async Task GetProjectsForUserAsync_ShouldReturnProjectsFromPermissionApiServiceFirstTime()
        {
            // Act
            var result = await _dut.GetProjectsForUserAsync(TestPlant, Oid);

            // Assert
            AssertProjects(result);
            _permissionApiServiceMock.Verify(p => p.GetAllOpenProjectsAsync(TestPlant), Times.Once);
            _meApiServiceMock.Verify(p => p.TracePlantAsync(TestPlant), Times.Once);
        }

        [TestMethod]
        public async Task GetProjectsForUserAsync_ShouldReturnProjectsFromCacheSecondTime()
        {
            await _dut.GetProjectsForUserAsync(TestPlant, Oid);
            // Act
            var result = await _dut.GetProjectsForUserAsync(TestPlant, Oid);

            // Assert
            AssertProjects(result);
            // since GetProjectsForUserAsyncAsync has been called twice, but GetAllProjectsAsync has been called once, the second Get uses cache
            _permissionApiServiceMock.Verify(p => p.GetAllOpenProjectsAsync(TestPlant), Times.Once);
            _meApiServiceMock.Verify(p => p.TracePlantAsync(TestPlant), Times.Once);
        }

        [TestMethod]
        public async Task GetContentRestrictionsForUser_ShouldReturnPermissionsFromPermissionApiServiceFirstTime()
        {
            // Act
            var result = await _dut.GetContentRestrictionsForUserAsync(TestPlant, Oid);

            // Assert
            AssertRestrictions(result);
            _permissionApiServiceMock.Verify(p => p.GetContentRestrictionsAsync(TestPlant), Times.Once);
        }

        [TestMethod]
        public async Task GetContentRestrictionsForUser_ShouldReturnPermissionsFromCacheSecondTime()
        {
            await _dut.GetContentRestrictionsForUserAsync(TestPlant, Oid);
            // Act
            var result = await _dut.GetContentRestrictionsForUserAsync(TestPlant, Oid);

            // Assert
            AssertRestrictions(result);
            // since GetContentRestrictionsForUserAsync has been called twice, but GetContentRestrictionsAsync has been called once, the second Get uses cache
            _permissionApiServiceMock.Verify(p => p.GetContentRestrictionsAsync(TestPlant), Times.Once);
        }

        [TestMethod]
        public async Task GetPermissionsForUser_ShouldThrowExceptionWhenOidIsEmpty()
            => await Assert.ThrowsExceptionAsync<Exception>(() => _dut.GetPermissionsForUserAsync(TestPlant, Guid.Empty));

        [TestMethod]
        public async Task GetProjectsForUserAsync_ShouldThrowExceptionWhenOidIsEmpty()
            => await Assert.ThrowsExceptionAsync<Exception>(() => _dut.GetProjectsForUserAsync(TestPlant, Guid.Empty));

        [TestMethod]
        public async Task GetContentRestrictionsForUser_ShouldThrowExceptionWhenOidIsEmpty()
            => await Assert.ThrowsExceptionAsync<Exception>(() => _dut.GetContentRestrictionsForUserAsync(TestPlant, Guid.Empty));

        [TestMethod]
        public void ClearAll_ShouldClearAllPermissionCaches()
        {
            // Arrange
            var cacheManagerMock = new Mock<ICacheManager>();
            var dut = new PermissionCache(
                cacheManagerMock.Object,
                _permissionApiServiceMock.Object,
                _meApiServiceMock.Object,
                new Mock<IOptionsSnapshot<CacheOptions>>().Object);
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
            Assert.AreEqual(Project1WithAccess, result.First());
            Assert.AreEqual(Project2WithAccess, result.Last());
        }

        private void AssertRestrictions(IList<string> result)
        {
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(Restriction1, result.First());
            Assert.AreEqual(Restriction2, result.Last());
        }
    }
}
