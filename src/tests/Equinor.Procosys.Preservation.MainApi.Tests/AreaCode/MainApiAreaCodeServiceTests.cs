using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.MainApi.AreaCode;
using Equinor.Procosys.Preservation.MainApi.Client;
using Equinor.Procosys.Preservation.MainApi.Plant;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.MainApi.Tests.AreaCode
{
    [TestClass]
    public class MainApiAreaCodeServiceTests
    {
        private const string _plant = "PCS$TESTPLANT";
        private Mock<IOptionsMonitor<MainApiOptions>> _mainApiOptions;
        private Mock<IBearerTokenApiClient> _mainApiClient;
        private Mock<IPlantApiService> _plantApiService;
        private List<ProcosysAreaCode> _resultWithNoItems;
        private List<ProcosysAreaCode> _resultWithThreeItems;

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

            _resultWithNoItems = new List<ProcosysAreaCode>();

            _resultWithThreeItems = new List<ProcosysAreaCode>
            {
                new ProcosysAreaCode
                {
                    Id = 1,
                    Code = "CodeA",
                    Description = "Description1",
                },
                new ProcosysAreaCode
                {
                    Id = 2,
                    Code = "CodeB",
                    Description = "Description2",
                },
                new ProcosysAreaCode
                {
                    Id = 3,
                    Code = "CodeC",
                    Description = "Description3",
                }
            };
        }

        [TestMethod]
        public async Task GetAreaCodes_ReturnsThreeAreaCodes()
        {
            // Arrange
            _mainApiClient
                .SetupSequence(x => x.QueryAndDeserialize<List<ProcosysAreaCode>>(It.IsAny<string>()))
                .Returns(Task.FromResult(_resultWithThreeItems));
            var dut = new MainApiAreaCodeService(_mainApiClient.Object, _plantApiService.Object, _mainApiOptions.Object);

            // Act
            var result = await dut.GetAreaCodes(_plant);

            // Assert
            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]
        public async Task GetAreaCodes_ReturnsNoAreaCodes()
        {
            // Arrange
            _mainApiClient
                .SetupSequence(x => x.QueryAndDeserialize<List<ProcosysAreaCode>>(It.IsAny<string>()))
                .Returns(Task.FromResult(_resultWithNoItems));
            var dut = new MainApiAreaCodeService(_mainApiClient.Object, _plantApiService.Object, _mainApiOptions.Object);

            // Act
            var result = await dut.GetAreaCodes(_plant);

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task GetAreaCodes_ThrowsException_WhenPlantIsInvalid()
        {
            var dut = new MainApiAreaCodeService(_mainApiClient.Object, _plantApiService.Object, _mainApiOptions.Object);

            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await dut.GetAreaCodes("INVALIDPLANT"));
        }
    }
}
