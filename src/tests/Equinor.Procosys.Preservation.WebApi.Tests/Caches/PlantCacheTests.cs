using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Infrastructure.Caching;
using Equinor.Procosys.Preservation.MainApi.Plant;
using Equinor.Procosys.Preservation.WebApi.Caches;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.WebApi.Tests.Caches
{
    [TestClass]
    public class PlantCacheTests
    {
        private readonly Guid _currentUserOid = new Guid("12345678-1234-1234-1234-123456789123");
        private Mock<IPlantApiService> _plantApiServiceMock;
        private Mock<ICurrentUserProvider> _currentUserProviderMock;
        private readonly string Plant1 = "P1";
        private readonly string Plant2 = "P2";

        private PlantCache _dut;

        [TestInitialize]
        public void Setup()
        {
            var optionsMock = new Mock<IOptionsMonitor<CacheOptions>>();
            optionsMock
                .Setup(x => x.CurrentValue)
                .Returns(new CacheOptions());

            _currentUserProviderMock = new Mock<ICurrentUserProvider>();
            _currentUserProviderMock.Setup(c => c.TryGetCurrentUserOid()).Returns(_currentUserOid);

            _plantApiServiceMock = new Mock<IPlantApiService>();
            _plantApiServiceMock.Setup(p => p.GetPlantsAsync()).Returns(Task.FromResult(
                new List<ProcosysPlant> {new ProcosysPlant {Id = Plant1}, new ProcosysPlant {Id = Plant2}}));

            _dut = new PlantCache(new CacheManager(), _currentUserProviderMock.Object, _plantApiServiceMock.Object, optionsMock.Object);
        }

        [TestMethod]
        public async Task GetPlantIdsForUserOid_ShouldReturnPlantIdsFromPlantApiServiceFirstTime()
        {
            // Act
            var result = await _dut.GetPlantIdsForUserOidAsync(_currentUserOid);

            // Assert
            AssertPlants(result);
            _plantApiServiceMock.Verify(p => p.GetPlantsAsync(), Times.Once);
        }

        [TestMethod]
        public async Task GetPlantsForUserOid_ShouldReturnPlantsFromCacheSecondTime()
        {
            await _dut.GetPlantIdsForUserOidAsync(_currentUserOid);

            // Act
            var result = await _dut.GetPlantIdsForUserOidAsync(_currentUserOid);

            // Assert
            AssertPlants(result);
            // since GetPlantIdsForUserOidAsync has been called twice, but GetPlantsAsync has been called once, the second Get uses cache
            _plantApiServiceMock.Verify(p => p.GetPlantsAsync(), Times.Once);
        }

        [TestMethod]
        public async Task IsValidPlantForCurrentUser_ShouldReturnTrue_WhenKnownPlant()
        {
            // Act
            var result = await _dut.IsValidPlantForCurrentUserAsync(Plant2);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task IsValidPlantForCurrentUser_ShouldReturnFalse_WhenUnknownPlant()
        {
            // Act
            var result = await _dut.IsValidPlantForCurrentUserAsync("XYZ");

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task IsValidPlantForCurrentUser_ShouldReturnPlantIdsFromPlantApiServiceFirstTime()
        {
            // Act
            await _dut.IsValidPlantForCurrentUserAsync(Plant2);

            // Assert
            _plantApiServiceMock.Verify(p => p.GetPlantsAsync(), Times.Once);
        }

        [TestMethod]
        public async Task IsValidPlantForCurrentUser_ShouldReturnPlantsFromCacheSecondTime()
        {
            await _dut.IsValidPlantForCurrentUserAsync("XYZ");
            // Act
            await _dut.IsValidPlantForCurrentUserAsync(Plant2);

            // Assert
            _plantApiServiceMock.Verify(p => p.GetPlantsAsync(), Times.Once);
        }

        [TestMethod]
        public async Task IsValidPlantForUser_ShouldReturnTrue_WhenKnownPlant()
        {
            // Act
            var result = await _dut.IsValidPlantForUserAsync(Plant2, _currentUserOid);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task IsValidPlantForUser_ShouldReturnFalse_WhenUnknownPlant()
        {
            // Act
            var result = await _dut.IsValidPlantForUserAsync("XYZ", _currentUserOid);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task IsValidPlantForUser_ShouldReturnPlantIdsFromPlantApiServiceFirstTime()
        {
            // Act
            await _dut.IsValidPlantForUserAsync(Plant2, _currentUserOid);

            // Assert
            _plantApiServiceMock.Verify(p => p.GetPlantsAsync(), Times.Once);
        }

        [TestMethod]
        public async Task IsValidPlantForUser_ShouldReturnPlantsFromCache()
        {
            await _dut.IsValidPlantForCurrentUserAsync("XYZ");
            await _dut.IsValidPlantForUserAsync("ABC", _currentUserOid);
            // Act
            await _dut.IsValidPlantForUserAsync(Plant2, _currentUserOid);

            // Assert
            _plantApiServiceMock.Verify(p => p.GetPlantsAsync(), Times.Once);
        }

        private void AssertPlants(IList<string> result)
        {
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(Plant1, result.First());
            Assert.AreEqual(Plant2, result.Last());
        }
    }
}
