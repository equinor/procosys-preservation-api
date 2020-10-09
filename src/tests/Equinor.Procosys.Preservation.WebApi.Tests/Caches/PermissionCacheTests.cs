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
        private readonly string Project1WithAccess = "P1";
        private readonly string Project2WithAccess = "P2";
        private readonly string ProjectWithoutAccess = "P3";
        private readonly string Restriction1 = "R1";
        private readonly string Restriction2 = "R2";

        [TestInitialize]
        public void Setup()
        {
            TimeService.SetProvider(new ManualTimeProvider(new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc)));

            _permissionApiServiceMock = new Mock<IPermissionApiService>();
            _permissionApiServiceMock.Setup(p => p.GetAllProjectsAsync(TestPlant))
                .Returns(Task.FromResult<IList<ProcosysProject>>(new List<ProcosysProject>
                {
                    new ProcosysProject {Name = Project1WithAccess, HasAccess = true},
                    new ProcosysProject {Name = Project2WithAccess, HasAccess = true},
                    new ProcosysProject {Name = ProjectWithoutAccess}
                }));
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
        public async Task GetProjectsForUser_ShouldReturnProjectsFromPermissionApiServiceFirstTime()
        {
            // Act
            var result = await _dut.GetProjectsForUserAsync(TestPlant, Oid);

            // Assert
            AssertProjects(result);
            _permissionApiServiceMock.Verify(p => p.GetAllProjectsAsync(TestPlant), Times.Once);
        }

        [TestMethod]
        public async Task GetProjectsForUser_ShouldReturnProjectsFromCacheSecondTime()
        {
            await _dut.GetProjectsForUserAsync(TestPlant, Oid);
            // Act
            var result = await _dut.GetProjectsForUserAsync(TestPlant, Oid);

            // Assert
            AssertProjects(result);
            // since GetProjectsForUserAsync has been called twice, but GetAllProjectsAsync has been called once, the second Get uses cache
            _permissionApiServiceMock.Verify(p => p.GetAllProjectsAsync(TestPlant), Times.Once);
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
        public async Task GetPermissionsForUser_ShouldThrowExceptionWhenOidIsEmty()
            => await Assert.ThrowsExceptionAsync<Exception>(() => _dut.GetPermissionsForUserAsync(TestPlant, Guid.Empty));

        [TestMethod]
        public async Task GetProjectNamesForUserOid_ShouldThrowExceptionWhenOidIsEmty()
            => await Assert.ThrowsExceptionAsync<Exception>(() => _dut.GetProjectsForUserAsync(TestPlant, Guid.Empty));

        [TestMethod]
        public async Task GetContentRestrictionsForUser_ShouldThrowExceptionWhenOidIsEmty()
            => await Assert.ThrowsExceptionAsync<Exception>(() => _dut.GetContentRestrictionsForUserAsync(TestPlant, Guid.Empty));

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
