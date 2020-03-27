using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.MainApi.Client;
using Equinor.Procosys.Preservation.MainApi.Discipline;
using Equinor.Procosys.Preservation.MainApi.Plant;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.MainApi.Tests.Discipline
{
    [TestClass]
    public class MainApiDisciplineServiceTests
    {
        private const string _plant = "PCS$TESTPLANT";
        private Mock<IOptionsMonitor<MainApiOptions>> _mainApiOptions;
        private Mock<IBearerTokenApiClient> _mainApiClient;
        private Mock<IPlantApiService> _plantApiService;
        private List<ProcosysDiscipline> _resultWithNoItems;
        private List<ProcosysDiscipline> _resultWithThreeItems;
        private ProcosysDiscipline _procosysDiscipline;
        private MainApiDisciplineService _dut;

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

            _resultWithNoItems = new List<ProcosysDiscipline>();

            _procosysDiscipline = new ProcosysDiscipline
            {
                Id = 1,
                Code = "CodeA",
                Description = "Description1",
            };
            _resultWithThreeItems = new List<ProcosysDiscipline>
            {
                _procosysDiscipline,
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
            _dut = new MainApiDisciplineService(_mainApiClient.Object, _plantApiService.Object, _mainApiOptions.Object);
        }

        [TestMethod]
        public async Task GetDisciplines_ShouldReturnsThreeDisciplines()
        {
            // Arrange
            _mainApiClient
                .SetupSequence(x => x.QueryAndDeserialize<List<ProcosysDiscipline>>(It.IsAny<string>()))
                .Returns(Task.FromResult(_resultWithThreeItems));

            // Act
            var result = await _dut.GetDisciplines(_plant);

            // Assert
            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]
        public async Task GetDisciplines_ShouldReturnsNoDisciplines()
        {
            // Arrange
            _mainApiClient
                .SetupSequence(x => x.QueryAndDeserialize<List<ProcosysDiscipline>>(It.IsAny<string>()))
                .Returns(Task.FromResult(_resultWithNoItems));
            // Act
            var result = await _dut.GetDisciplines(_plant);

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task GetDisciplines_ShouldThrowException_WhenPlantIsInvalid()
            => await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await _dut.GetDisciplines("INVALIDPLANT"));

        [TestMethod]
        public async Task GetDiscipline_ShouldThrowException_WhenPlantIsInvalid()
            => await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await _dut.GetDiscipline("INVALIDPLANT", "C"));

        [TestMethod]
        public async Task GetDiscipline_ShouldReturnDiscipline()
        {
            // Arrange
            _mainApiClient
                .SetupSequence(x => x.QueryAndDeserialize<ProcosysDiscipline>(It.IsAny<string>()))
                .Returns(Task.FromResult(_procosysDiscipline));

            // Act
            var result = await _dut.GetDiscipline(_plant, _procosysDiscipline.Code);

            // Assert
            Assert.AreEqual(_procosysDiscipline.Code, result.Code);
            Assert.AreEqual(_procosysDiscipline.Description, result.Description);
        }
    }
}
