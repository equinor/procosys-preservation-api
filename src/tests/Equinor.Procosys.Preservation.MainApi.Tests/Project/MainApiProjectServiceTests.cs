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
        private Mock<IPlantCache> _plantCache;
        private ProcosysProject _result;
        private string _name = "NameA";
        private string _description = "Description1";
        private MainApiProjectService _dut;

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

            _result = new ProcosysProject {Id = 1, Name = _name, Description = _description};
            _dut = new MainApiProjectService(_mainApiClient.Object, _plantCache.Object, _mainApiOptions.Object);
        }

        [TestMethod]
        public async Task GetProject_ReturnsProject()
        {
            // Arrange
            _mainApiClient
                .SetupSequence(x => x.QueryAndDeserialize<ProcosysProject>(It.IsAny<string>()))
                .Returns(Task.FromResult(_result));

            // Act
            var result = await _dut.GetProjectAsync(_plant, _name);

            // Assert
            Assert.AreEqual(_name, result.Name);
            Assert.AreEqual(_description, result.Description);
        }
    }
}
