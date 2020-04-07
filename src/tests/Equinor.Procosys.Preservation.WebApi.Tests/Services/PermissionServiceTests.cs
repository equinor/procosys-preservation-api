using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Infrastructure.Caching;
using Equinor.Procosys.Preservation.MainApi.Permission;
using Equinor.Procosys.Preservation.MainApi.Project;
using Equinor.Procosys.Preservation.WebApi.Services;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.WebApi.Tests.Services
{
    [TestClass]
    public class PermissionServiceTests
    {
        private PermissionService _dut;
        private readonly Guid Oid = new Guid("{3BFB54C7-91E2-422E-833F-951AD07FE37F}");
        private Mock<IPermissionApiService> _permissionApiServiceMock;
        private Mock<IProjectApiService> _projectApiServiceMock;
        private readonly string TestPlant = "TestPlant";

        [TestInitialize]
        public void Setup()
        {
            var plantProviderMock = new Mock<IPlantProvider>();
            plantProviderMock.SetupGet(p => p.Plant).Returns(TestPlant);

            _permissionApiServiceMock = new Mock<IPermissionApiService>();
            _permissionApiServiceMock.Setup(p => p.GetPermissionsAsync(TestPlant))
                .Returns(Task.FromResult<IList<string>>(new List<string> {"A", "B"}));

            _projectApiServiceMock = new Mock<IProjectApiService>();
            _projectApiServiceMock.Setup(p => p.GetProjectsAsync(TestPlant))
                .Returns(Task.FromResult<IList<ProcosysProject>>(new List<ProcosysProject>
                {
                    new ProcosysProject {Name = "P1"}, new ProcosysProject {Name = "P2"}
                }));

            var optionsMock = new Mock<IOptionsMonitor<PermissionOptions>>();
            optionsMock
                .Setup(x => x.CurrentValue)
                .Returns(new PermissionOptions());


            _dut = new PermissionService(plantProviderMock.Object, new CacheManager(), _projectApiServiceMock.Object, _permissionApiServiceMock.Object, optionsMock.Object);
        }

        [TestMethod]
        public async Task GetPermissionsForUserOid_ShouldReturnPermissionsFromPermissionApiServiceFirstTime()
        {
            // Act
            var result = await _dut.GetPermissionsForUserOidAsync(Oid);

            // Assert
            AssertPermissions(result);
            _permissionApiServiceMock.Verify(p => p.GetPermissionsAsync(TestPlant), Times.Once);
        }

        [TestMethod]
        public async Task GetPermissionsForUserOid_ShouldReturnPermissionsFromCacheSecondTime()
        {
            await _dut.GetPermissionsForUserOidAsync(Oid);
            // Act
            var result = await _dut.GetPermissionsForUserOidAsync(Oid);

            // Assert
            AssertPermissions(result);
            // since GetPermissionsForUserOidAsync has been called twice, but GetPermissionsAsync has been called once, the second Get uses cache
            _permissionApiServiceMock.Verify(p => p.GetPermissionsAsync(TestPlant), Times.Once);
        }

        [TestMethod]
        public async Task GetProjectsForUserOid_ShouldReturnProjectsFromPermissionApiServiceFirstTime()
        {
            // Act
            var result = await _dut.GetProjectsForUserOidAsync(Oid);

            // Assert
            AssertProjects(result);
            _projectApiServiceMock.Verify(p => p.GetProjectsAsync(TestPlant), Times.Once);
        }

        [TestMethod]
        public async Task GetProjectsForUserOid_ShouldReturnProjectsFromCacheSecondTime()
        {
            await _dut.GetProjectsForUserOidAsync(Oid);
            // Act
            var result = await _dut.GetProjectsForUserOidAsync(Oid);

            // Assert
            AssertProjects(result);
            // since GetProjectsForUserOidAsync has been called twice, but GetProjectsAsync has been called once, the second Get uses cache
            _projectApiServiceMock.Verify(p => p.GetProjectsAsync(TestPlant), Times.Once);
        }

        private static void AssertPermissions(IList<string> result)
        {
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("A", result.First());
            Assert.AreEqual("B", result.Last());
        }

        private static void AssertProjects(IList<string> result)
        {
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("P1", result.First());
            Assert.AreEqual("P2", result.Last());
        }
    }
}
