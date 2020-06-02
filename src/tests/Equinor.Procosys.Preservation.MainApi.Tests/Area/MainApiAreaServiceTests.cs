using System;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.MainApi.Area;
using Equinor.Procosys.Preservation.MainApi.Client;
using Equinor.Procosys.Preservation.MainApi.Plant;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.MainApi.Tests.Area
{
    [TestClass]
    public class MainApiAreaServiceTests
    {
        private const string _plant = "PCS$TESTPLANT";
        private Mock<IOptionsMonitor<MainApiOptions>> _mainApiOptions;
        private Mock<IBearerTokenApiClient> _mainApiClient;
        private Mock<IPlantCache> _plantCache;
        private MainApiAreaService _dut;
        private ProcosysArea _procosysArea;

        [TestInitialize]
        public void Setup()
        {
            _mainApiOptions = new Mock<IOptionsMonitor<MainApiOptions>>();
            _mainApiOptions
                .Setup(x => x.CurrentValue)
                .Returns(new MainApiOptions { ApiVersion = "4.0", BaseAddress = "http://example.com" });
            _mainApiClient = new Mock<IBearerTokenApiClient>();
            _plantCache = new Mock<IPlantCache>();
            _plantCache
                .Setup(x => x.IsValidPlantForCurrentUserAsync(_plant))
                .Returns(Task.FromResult(true));

            _procosysArea = new ProcosysArea
            {
                Id = 1,
                Code = "CodeA",
                Description = "Description1",
            };
           
            _dut = new MainApiAreaService(_mainApiClient.Object, _plantCache.Object, _mainApiOptions.Object);
        }

        [TestMethod]
        public async Task GetAreaCode_ThrowsException_WhenPlantIsInvalid()
            => await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await _dut.GetAreaAsync("INVALIDPLANT", "C"));

        [TestMethod]
        public async Task GetAreaCode_ReturnsAreaCode()
        {
            // Arrange
            _mainApiClient
                .SetupSequence(x => x.QueryAndDeserialize<ProcosysArea>(It.IsAny<string>()))
                .Returns(Task.FromResult(_procosysArea));
            // Act
            var result = await _dut.GetAreaAsync(_plant, _procosysArea.Code);

            // Assert
            Assert.AreEqual(_procosysArea.Code, result.Code);
            Assert.AreEqual(_procosysArea.Description, result.Description);
        }
    }
}
