using System;
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
        private Mock<IPlantCache> _plantCache;
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
            _plantCache = new Mock<IPlantCache>();
            _plantCache
                .Setup(x => x.IsValidPlantForCurrentUserAsync(_plant))
                .Returns(Task.FromResult(true));

            _procosysDiscipline = new ProcosysDiscipline
            {
                Id = 1,
                Code = "CodeA",
                Description = "Description1",
            };
           
            _dut = new MainApiDisciplineService(_mainApiClient.Object, _plantCache.Object, _mainApiOptions.Object);
        }

        [TestMethod]
        public async Task TryGetDiscipline_ShouldThrowException_WhenPlantIsInvalid()
            => await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await _dut.TryGetDisciplineAsync("INVALIDPLANT", "C"));

        [TestMethod]
        public async Task TryGetDiscipline_ShouldReturnDiscipline()
        {
            // Arrange
            _mainApiClient
                .SetupSequence(x => x.TryQueryAndDeserializeAsync<ProcosysDiscipline>(It.IsAny<string>()))
                .Returns(Task.FromResult(_procosysDiscipline));

            // Act
            var result = await _dut.TryGetDisciplineAsync(_plant, _procosysDiscipline.Code);

            // Assert
            Assert.AreEqual(_procosysDiscipline.Code, result.Code);
            Assert.AreEqual(_procosysDiscipline.Description, result.Description);
        }
    }
}
