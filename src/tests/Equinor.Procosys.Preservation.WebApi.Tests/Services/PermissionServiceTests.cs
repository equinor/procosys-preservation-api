using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Infrastructure.Caching;
using Equinor.Procosys.Preservation.MainApi.Permission;
using Equinor.Procosys.Preservation.MainApi.Plant;
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
        private Mock<IPlantApiService> _plantApiServiceMock;
        private Mock<IPermissionApiService> _permissionApiServiceMock;
        private Mock<IProjectApiService> _projectApiServiceMock;
        private readonly string TestPlant = "TestPlant";
        private readonly string Plant1 = "X";
        private readonly string Plant2 = "Y";
        private readonly string Permission1 = "A";
        private readonly string Permission2 = "B";
        private readonly string Project1 = "P1";
        private readonly string Project2 = "P2";
        private readonly string Restriction1 = "R1";
        private readonly string Restriction2 = "R2";

        [TestInitialize]
        public void Setup()
        {
            var plantProviderMock = new Mock<IPlantProvider>();
            plantProviderMock.SetupGet(p => p.Plant).Returns(TestPlant);

            _plantApiServiceMock = new Mock<IPlantApiService>();
            _plantApiServiceMock.Setup(p => p.GetPlantsAsync())
                .Returns(Task.FromResult(new List<ProcosysPlant>
                {
                    new ProcosysPlant {Id = Plant1}, new ProcosysPlant {Id = Plant2}
                }));

            _permissionApiServiceMock = new Mock<IPermissionApiService>();
            _permissionApiServiceMock.Setup(p => p.GetPermissionsAsync(TestPlant))
                .Returns(Task.FromResult<IList<string>>(new List<string> {Permission1, Permission2}));
            _permissionApiServiceMock.Setup(p => p.GetContentRestrictionsAsync(TestPlant))
                .Returns(Task.FromResult<IList<string>>(new List<string> {Restriction1, Restriction2}));

            _projectApiServiceMock = new Mock<IProjectApiService>();
            _projectApiServiceMock.Setup(p => p.GetProjectsAsync(TestPlant))
                .Returns(Task.FromResult<IList<ProcosysProject>>(new List<ProcosysProject>
                {
                    new ProcosysProject {Name = Project1}, new ProcosysProject {Name = Project2}
                }));

            var optionsMock = new Mock<IOptionsMonitor<PermissionOptions>>();
            optionsMock
                .Setup(x => x.CurrentValue)
                .Returns(new PermissionOptions());


            _dut = new PermissionService(
                plantProviderMock.Object, 
                new CacheManager(),
                _plantApiServiceMock.Object,
                _projectApiServiceMock.Object,
                _permissionApiServiceMock.Object,
                optionsMock.Object);
        }

        [TestMethod]
        public async Task GetPlantsForUserOid_ShouldReturnPlantsFromPlantApiServiceFirstTime()
        {
            // Act
            var result = await _dut.GetPlantsForUserOidAsync(Oid);

            // Assert
            AssertPlants(result);
            _plantApiServiceMock.Verify(p => p.GetPlantsAsync(), Times.Once);
        }

        [TestMethod]
        public async Task GetPlantsForUserOid_ShouldReturnPlantsFromCacheSecondTime()
        {
            await _dut.GetPlantsForUserOidAsync(Oid);
            // Act
            var result = await _dut.GetPlantsForUserOidAsync(Oid);

            // Assert
            AssertPlants(result);
            // since GetPlantsForUserOidAsync has been called twice, but GetPlantsAsync has been called once, the second Get uses cache
            _plantApiServiceMock.Verify(p => p.GetPlantsAsync(), Times.Once);
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

        [TestMethod]
        public async Task GetContentRestrictionsForUserOid_ShouldReturnPermissionsFromPermissionApiServiceFirstTime()
        {
            // Act
            var result = await _dut.GetContentRestrictionsForUserOidAsync(Oid);

            // Assert
            AssertRestrictions(result);
            _permissionApiServiceMock.Verify(p => p.GetContentRestrictionsAsync(TestPlant), Times.Once);
        }

        [TestMethod]
        public async Task GetContentRestrictionsForUserOid_ShouldReturnPermissionsFromCacheSecondTime()
        {
            await _dut.GetContentRestrictionsForUserOidAsync(Oid);
            // Act
            var result = await _dut.GetContentRestrictionsForUserOidAsync(Oid);

            // Assert
            AssertRestrictions(result);
            // since GetContentRestrictionsForUserOidAsync has been called twice, but GetContentRestrictionsAsync has been called once, the second Get uses cache
            _permissionApiServiceMock.Verify(p => p.GetContentRestrictionsAsync(TestPlant), Times.Once);
        }

        
        private void AssertPlants(IList<string> result)
        {
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(Plant1, result.First());
            Assert.AreEqual(Plant2, result.Last());
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
