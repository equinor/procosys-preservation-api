using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.MainApi.Client;
using Equinor.Procosys.Preservation.MainApi.Discipline;
using Equinor.Procosys.Preservation.MainApi.Plant;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.MainApi.Tests.Discipline
{
    [TestClass]
    public class MainApiDisciplineServiceTests
    {
        private const string _plant = "PCS$TESTPLANT";
        private Mock<ILogger<MainApiDisciplineService>> _logger;
        private Mock<IOptionsMonitor<MainApiOptions>> _mainApiOptions;
        private Mock<IBearerTokenApiClient> _mainApiClient;
        private Mock<IPlantApiService> _plantApiService;
        private List<ProcosysDiscipline> _resultWithNoItems;
        private List<ProcosysDiscipline> _resultWithThreeItems;

        [TestInitialize]
        public void Setup()
        {
            _logger = new Mock<ILogger<MainApiDisciplineService>>();
            _mainApiOptions = new Mock<IOptionsMonitor<MainApiOptions>>();
            _mainApiOptions
                .Setup(x => x.CurrentValue)
                .Returns(new MainApiOptions { ApiVersion = "4.0", BaseAddress = "http://example.com" });
            _mainApiClient = new Mock<IBearerTokenApiClient>();
            _plantApiService = new Mock<IPlantApiService>();
            _plantApiService
                .Setup(x => x.IsPlantValidAsync(_plant))
                .Returns(Task.FromResult(true));

            _resultWithNoItems = new List<ProcosysDiscipline>();

            _resultWithThreeItems = new List<ProcosysDiscipline>
            {
                new ProcosysDiscipline
                {
                    Id = 1,
                    Code = "CodeA",
                    Description = "Description1",
                },
                new ProcosysDiscipline
                {
                    Id = 2,
                    Code = "CodeB",
                    Description = "Description2",
                },
                new ProcosysDiscipline
                {
                    Id = 3,
                    Code = "CodeC",
                    Description = "Description3",
                }
            };
        }

        [TestMethod]
        public async Task GetDisciplines_ReturnsThreeDisciplines_TestAsync()
        {
            // Arrange
            _mainApiClient
                .SetupSequence(x => x.QueryAndDeserialize<List<ProcosysDiscipline>>(It.IsAny<string>()))
                .Returns(Task.FromResult(_resultWithThreeItems));
            var dut = new MainApiDisciplineService(_mainApiClient.Object, _plantApiService.Object, _mainApiOptions.Object, _logger.Object);

            // Act
            var result = await dut.GetDisciplines(_plant);

            // Assert
            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]
        public async Task GetDisciplines_ReturnsNoDisciplines_TestAsync()
        {
            // Arrange
            _mainApiClient
                .SetupSequence(x => x.QueryAndDeserialize<List<ProcosysDiscipline>>(It.IsAny<string>()))
                .Returns(Task.FromResult(_resultWithNoItems));
            var dut = new MainApiDisciplineService(_mainApiClient.Object, _plantApiService.Object, _mainApiOptions.Object, _logger.Object);

            // Act
            var result = await dut.GetDisciplines(_plant);

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task GetDisciplines_ThrowsException_WhenPlantIsInvalid_TestAsync()
        {
            var dut = new MainApiDisciplineService(_mainApiClient.Object, _plantApiService.Object, _mainApiOptions.Object, _logger.Object);

            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await dut.GetDisciplines("INVALIDPLANT"));
        }
    }
}
