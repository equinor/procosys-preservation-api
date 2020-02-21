using System;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.MainApi.Project;
using Equinor.Procosys.Preservation.MainApi.Client;
using Equinor.Procosys.Preservation.MainApi.Plant;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.MainApi.Tests.Project
{
    [TestClass]
    public class MainApiProjectServiceTests
    {
        private const string _plant = "PCS$TESTPLANT";
        private Mock<IOptionsMonitor<MainApiOptions>> _mainApiOptions;
        private Mock<IBearerTokenApiClient> _mainApiClient;
        private Mock<IPlantApiService> _plantApiService;
        private ProcosysProject _result;
        private string _name = "NameA";
        private string _description = "Description1";

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

            _result = new ProcosysProject {Id = 1, Name = _name, Description = _description,};
        }

        [TestMethod]
        public async Task GetProjectCodes_ReturnsThreeProjectCodes()
        {
            // Arrange
            _mainApiClient
                .SetupSequence(x => x.QueryAndDeserialize<ProcosysProject>(It.IsAny<string>()))
                .Returns(Task.FromResult(_result));
            var dut = new MainApiProjectService(_mainApiClient.Object, _plantApiService.Object, _mainApiOptions.Object);

            // Act
            var result = await dut.GetProject(_plant, _name);

            // Assert
            Assert.AreEqual(_name, result.Name);
            Assert.AreEqual(_description, result.Description);
        }

        [TestMethod]
        public async Task GetProjectCodes_ThrowsException_WhenPlantIsInvalid()
        {
            var dut = new MainApiProjectService(_mainApiClient.Object, _plantApiService.Object, _mainApiOptions.Object);

            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await dut.GetProject("INVALIDPLANT", ""));
        }
    }
}
