using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.MainApi.Client;
using Equinor.ProCoSys.Preservation.MainApi.Plant;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.MainApi.Tests.Plant
{
    [TestClass]
    public class MainApiPlantServiceTests
    {
        private MainApiPlantService _dut;
        private readonly string _plantId = "PCS$AASTA_HANSTEEN";
        private readonly string _plantTitle = "AastaHansteen";

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            var mainApiOptions = new Mock<IOptionsMonitor<MainApiOptions>>();
            mainApiOptions
                .Setup(x => x.CurrentValue)
                .Returns(new MainApiOptions { ApiVersion = "4.0", BaseAddress = "http://example.com" });
            var mainApiClient = new Mock<IBearerTokenApiClient>();
            mainApiClient
                .Setup(x => x.QueryAndDeserializeAsync<List<ProcosysPlant>>(It.IsAny<string>()))
                .Returns(Task.FromResult(new List<ProcosysPlant>
                {
                    new ProcosysPlant { Id = _plantId, Title = _plantTitle },
                    new ProcosysPlant { Id = "PCS$ASGARD", Title = "Åsgard" },
                    new ProcosysPlant { Id = "PCS$ASGARD_A", Title = "ÅsgardA" },
                    new ProcosysPlant { Id = "PCS$ASGARD_B", Title = "ÅsgardB" },
                }));

            _dut = new MainApiPlantService(mainApiClient.Object, mainApiOptions.Object);
        }

        [TestMethod]
        public async Task GetAllPlants_ShouldReturnCorrectNumberOfPlants()
        {
            // Act
            var result = await _dut.GetAllPlantsAsync();

            // Assert
            Assert.AreEqual(4, result.Count);
        }

        [TestMethod]
        public async Task GetAllPlants_ShouldSetsCorrectProperties()
        {
            // Act
            var result = await _dut.GetAllPlantsAsync();

            // Assert
            var plant = result.First();
            Assert.AreEqual(_plantId, plant.Id);
            Assert.AreEqual(_plantTitle, plant.Title);
        }
    }
}
