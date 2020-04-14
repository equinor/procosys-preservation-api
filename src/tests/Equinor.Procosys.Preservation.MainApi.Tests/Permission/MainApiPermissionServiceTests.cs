using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.MainApi.Client;
using Equinor.Procosys.Preservation.MainApi.Permission;
using Equinor.Procosys.Preservation.MainApi.Plant;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.MainApi.Tests.Permission
{
    [TestClass]
    public class MainApiPermissionServiceTests
    {
        private const string _plant = "PCS$TESTPLANT";
        private Mock<IOptionsMonitor<MainApiOptions>> _mainApiOptions;
        private Mock<IBearerTokenApiClient> _mainApiClient;
        private Mock<IPlantApiService> _plantApiService;
        private MainApiPermissionService _dut;

        [TestInitialize]
        public void Setup()
        {
            _mainApiOptions = new Mock<IOptionsMonitor<MainApiOptions>>();
            _mainApiOptions
                .Setup(x => x.CurrentValue)
                .Returns(new MainApiOptions { ApiVersion = "4.0", BaseAddress = "http://example.com" });
            _mainApiClient = new Mock<IBearerTokenApiClient>();
            _plantApiService = new Mock<IPlantApiService>();
            _plantApiService
                .Setup(x => x.IsPlantValidAsync(_plant))
                .Returns(Task.FromResult(true));

            _dut = new MainApiPermissionService(_mainApiClient.Object, _plantApiService.Object, _mainApiOptions.Object);
        }

        [TestMethod]
        public async Task GetPermissions_ReturnsThreePermissions()
        {
            // Arrange
            _mainApiClient
                .SetupSequence(x => x.QueryAndDeserialize<List<string>>(It.IsAny<string>()))
                .Returns(Task.FromResult(new List<string>{ "A", "B", "C" }));
            // Act
            var result = await _dut.GetPermissionsAsync(_plant);

            // Assert
            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]
        public async Task GetPermissions_ReturnsNoPermissions()
        {
            // Arrange
            _mainApiClient
                .SetupSequence(x => x.QueryAndDeserialize<List<string>>(It.IsAny<string>()))
                .Returns(Task.FromResult(new List<string>()));
            // Act
            var result = await _dut.GetPermissionsAsync(_plant);

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task GetPermissions_ThrowsException_WhenPlantIsInvalid()
            => await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await _dut.GetPermissionsAsync("INVALIDPLANT"));
    }
}
